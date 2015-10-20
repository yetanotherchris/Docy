![](https://github.com/yetanotherchris/Docy/blob/master/logo.png)

Docy is a tool for producing the MSDN script-free look for .NET documentation, using XML comments. It produces static html, wiki markup and also has an MVC site. It uses the Razor view engine and Jolt.

The project uses the following technologies:
* .NET 4.0
* MVC ASP.NET 3/Razor (optional - required for the website)
* Jolt framework
* Razorengine

You therefore need the .NET 4 framework to run Docy. 

###Docy vs Sandcastle or NDoc3. 
Docy's primary goal is generating the script-free MSDN look, and providing an API that is quick and easy to use. It is "feature-light".
It does not produce HTML help, MSDN deep-tree look nor a whole raft of features that Docproject provides you with. 
It is aimed at small/mid-size frameworks and not giant ones like the .NET framework, infragistics etc.

### Current Known Issues
* At present Generics support is buggy (and comments are not retrieved). I'm looking for a Generics guru to help with this. Please contact me via codeplex if you can help out.
* Static methods and extension methods don't have their summaries included.
* Static methods aren't shown with the appropriate icon
* Default constructors appear
* No exceptions appear
* Syntax for various languages is not present

###Output###

Docy has two downloads. One is a command line tool for producing static content, the other is a website. Combined, docy can produce:

1. Static HTML
2. Wiki (Creole, MediaWiki or Markdown) pages. This format is obviously limited to text and links-only
3. Website using ASP.NET MVC 3.

At present the output is the script-free look. However if you are familiar with the http://haacked.com/archive/2011/01/06/razor-syntax-quick-reference.aspx it is easy to customise the views. The static HTML and wiki text is produced using Razor syntax.

### Screenshots

![](https://github.com/yetanotherchris/Docy/blob/master/screenshot1.png)
![](https://github.com/yetanotherchris/Docy/blob/master/screenshot2.png)

###Roll your own###

The API is easy to use if you want to make your own generator:

    DocParser parser = new DocParser();
    IEnumerable<Namespace> namespaces = parser.Parse(new string[]{
    	@"C:\somefolder\bin\Release\Roadkill.Core.dll",
    	@"C:\somefolder\bin\Release\NHibernate.dll"
    });
    
    foreach (Namespace nameSpace in namespaces)
    {
    	Console.WriteLine(nameSpace.Name);
    	
    	Console.WriteLine("Classes:");
    	foreach (TypeBase type in namespaces.Classes)
    	{
    		Console.WriteLine(type.Name +":");
    		Console.WriteLine(type.Summary);
    		Console.WriteLine();
    		Console.WriteLine(type.Remarks);
    	}
    }

