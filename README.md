# DotNetLocalizer

AppVeyor:

[![Build status](https://ci.appveyor.com/api/projects/status/6lshcksp3oh9craj?svg=true)](https://ci.appveyor.com/project/Baklap4/dotnetlocalizer)

# Why

Since ASP.NET Core doesn't provide localization in other formats than `.RESX`.
Therefore I made a core package from which can extended from to support multiple formats.
Currently supported formats are
- [x] JSON
- [ ] YAML

# Installation / Configuration

## Nuget
Install the desired ASP.NET Core extension package:

```PowerShell
Install-Package DotNetLocalizer.Json;
```

## ASP.NET Middleware And Default Setup
Register the localizer in the service collection and set the options:
```cs
public void ConfigureServices(IServiceCollection services)
{
	// Add specific localization middleware
	services.AddJsonLocalization();
	
	// Add Mvc services to also support localization in views and DataAnnotation.
	services.AddMvc()
		.AddDataAnnotationsLocalization()
		.AddViewLocalization();
}
```

Set your RequestLocalizationOptions:
```cs
public void Configure(IApplicationBuilder app)
{
	var supportedCultures = new[]
	{
		new CultureInfo("en-US"),
		new CultureInfo("nl-NL")
	};
	app.UseRequestLocalization(new RequestLocalizationOptions
	{
	   DefaultRequestCulture = new RequestCulture("en"),

	   // Formatting numbers, dates, etc.
	   SupportedCultures = supportedCultures,

	   // UI strings that we have localized.
	   SupportedUICultures = supportedCultures
	});

	app.UseStaticFiles();

	app.UseMvc(routes => routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}"));
}
```

### ASP.NET Middleware Custom Configuration
One can also add a custom folder which contains all the localization files:
```cs
public void ConfigureServices(IServiceCollection services)
{
	// Add specific localization middleware where files are listed in '$PROJECT_ROOT/Some/Other/Location'
	services.AddJsonLocalization(options =>
	{
		options.ResourcesPath = "Some/Other/Location";
	});
	
	// Add Mvc services to also support localization in views and DataAnnotation.
	services.AddMvc()
		.AddDataAnnotationsLocalization()
		.AddViewLocalization();
}
```

### Use it in your ASP.NET Core application
#### Controllers
Within controllers one can easily inject this service as following:
```cs
public class HomeController : Controller
{
	private readonly Microsoft.Extensions.Localization.IStringLocalizer<HomeController> localizer;

	public HomeController(Microsoft.Extensions.Localization.IStringLocalizer<HomeController> localizer)
	{
		this.localizer = localizer;
	}
	
	public IActionResult Index()
	{
		// Getting localized string by key
		var localizedString = this.localizer["SomeKey"];
		
		// This will get a localized string with extra object values (filled in the string)
		var localizedStringWithData = this.localizer.GetString("SomeKeyWithObjects", "Object 1", "Object N");
		return this.View(localizedString);
	}
}
```

#### Views
Within Views one can easily inject this service as following:
```cshtml
@using Microsoft.AspNetCore.Mvc.Localization
@inject IViewLocalizer localizer
<div>
	Key 'SomeKey' => @localizer["SomeKey"]<br/>
    Key 'OtherKey' => @localizer["OtherKey"]<br/>
    Key 'OtherKeyWithObject' => @localizer.GetString("SomeKeyWithObjects", "Object 1", "Object N")<br/>
</div>
```

#### DataAnnotations
Within ViewModels one can easily use localization for example with error messages:
```cs
public class UserViewModel
{
	[Required(ErrorMessage = "UserNameRequired")]
	public string UserName { get; set; }
}
```

### Missing Strings
One hell of a job is to keep track of which items have been localized and which haven't.
DotNetLocalizer.Core has an extension method called `GetMissingLocalizedStrings()`.
This method can be called directly from `IStringLocalizer<T>` and will give you a `System.IO.FileStream` with all the missing strings from the current language.
I find it quite handy to make this accessable through an admin panel. 
So I'm injecting a class everytime I use this package to keep track of the missing strings.
If you don't include the below class one can find the missing strings per language in the default or given folder: **`$PROJECT_ROOT/Localization/MissingStrings.{language}.json`**
```cs
public class AdminController : Controller
{
	private readonly IStringLocalizer<AdminController> localizer;
	private readonly IEnumerable<CultureInfo> cultureList;

	public AdminController(IStringLocalizer<AdminController> localizer)
	{
		this.localizer = localizer;
		this.cultureList = new List<CultureInfo>
						   {
							   new CultureInfo("en-US"),
							   new CultureInfo("nl-NL"),
							   new CultureInfo("de-DE")
						   };
	}

	public IActionResult MissingTranslations()
	{
		var str = string.Empty;
		foreach (var cultureInfo in this.cultureList)
		{
			var filestream = this.localizer.GetMissingLocalizedStrings(null, cultureInfo);
			if (filestream == null)
			{
				continue;
			}
			str += $"{cultureInfo.TwoLetterISOLanguageName}\n";
			str += new StreamReader(filestream).ReadToEnd();
			str += "\n\n";
		}
		return this.Ok(str);
	}
}
```

# Supported Formats

## Default Location
The default location for this localizer is `$PROJECT_ROOT/Localization/`.
If no custom location is given the localizer will search for this directory and then loop over the files found there.

## JSON

### Filename format
Each file has a name in the format of `Strings.{language}.json`.
This means if you want to have a Dutch translation you'll have to add a file with the name `Strings.nl.json`.
Right now (as we didn't need it yet) it's not taking the culture in account, so for example English will be `Strings.en.json`.

### File contents
A prerequisite is that the json file is well-formatted.
To validate your JSON file try out the [JSON Formatter Website](https://jsonformatter.curiousconcept.com).
An example JSON file is added as reference here
```json
{
  "ComplexListObject": {
    "List": [
      {
        "Name": "Name"
      },
      {
        "Name": "Name Two"
      }
    ]
  },
  "ComplexListObjectMissingListItems": {
    "List": [
      {
        "Name": "Name"
      },
      {
        "Name": "Name Two"
      }
    ]
  },
  "Language": "Language",
  "NestedObject": {
    "NestedProperty": "Een geneste eigenschap"
  },
  "NestedObjectMissingChilds": {

  },
  "NotFoundKeyInCurrentCultureButInOtherFallback": "Key not found in current culture ('{0}') but found in other culture (fallback option)",
  "ObjectExampleKey": "Give me an object: '{0}'",
  "OtherKey": "Other key",
  "RequestCultureProvider": "Request Culture Provider",
  "SomeKey": "Some key"
}
```
## YAML

### Filename format
Each file has a name in the format of `Strings.{language}.yml`.
This means if you want to have a Dutch translation you'll have to add a file with the name `Strings.nl.yaml`.
Right now (as we didn't need it yet) it's not taking the culture in account, so for example English will be `Strings.en.yaml`.

### File contents
A prerequisite is that the YAML file is well-formatted.
To validate your YAML file try out the [YAML Formatter Website](http://www.yamllint.com/).
An example YAML file is added as reference here