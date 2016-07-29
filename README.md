# Sitecore.React
===

A ReactJS module for building Sitecore components with [React](https://facebook.github.io/react/) and [ReactJS.NET](reactjs.net).

The module has 2 parts. A Sitecore package that installs the required `JsxControllerRendering` and `JxsViewRendering` templates, and the Nuget package for use in your projects. This adds the required pipeline processors and components to render the React Jsx files as Sitecore renderings.

Features
---
* On the fly Jsx to JavaScript compilation via [Babel](http://babeljs.io/) and [ReactJS.NET](http://reactjs.net)
* Server-side component rendering. Initial renders are super-fast and great for SEO
* Full support for datasources, personalization and testing

Getting Started
---

1. Install the  NuGet package
```
Install-Package Sitecore.React
```

2. Install the [Sitecore package]()
3. Create your `JsxControllerRendering` controller and action
```c#
public SampleReactController : Controller 
{
	public ActionResult SampleReactRendering 
	{
		var data = new {
			Title = FieldRenderer(Sitecore.Context.Item, "Title"),
			Body = FieldRenderer(Sitecore.Context.Item, "Body")
		};

		return this.React("~/views/react/SampleReactRendering.jsx", data);
	}
}
```

4. Create your Jsx component
```javascript
var SampleReactRendering = React.createClass({
    render: function() {
        return (
            <div>
                <h1 dangerouslySetInnerHTML={{__html: this.props.data.Title}}></h1>
                <div dangerouslySetInnerHTML={{__html: this.props.data.Body}}></div>
            </div>
        );
    }
});
```

5. Create the rendering item in Sitecore and assign to an items presentation
6. Add the React JavaScript links and the Jsx bundle to your main layout

```cshtml
  <script src="//fb.me/react-15.0.1.js"></script>
  <script src="//fb.me/react-dom-15.0.1.js"></script>
  @Scripts.Render("~/bundles/react")
```

7. Have fun