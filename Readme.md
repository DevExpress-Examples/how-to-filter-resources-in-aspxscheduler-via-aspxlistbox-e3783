# How to filter resources in ASPxScheduler via ASPxListBox


<p>This example illustrates how to implement resource filtering at the datasource level via a checked list box that is represented by the <a href="http://documentation.devexpress.com/#AspNet/clsDevExpressWebASPxEditorsASPxListBoxtopic">ASPxListBox Class</a> with the <a href="http://documentation.devexpress.com/#AspNet/DevExpressWebASPxEditorsASPxListBox_SelectionModetopic">ASPxListBox.SelectionMode Property</a> set to <strong>CheckColumn</strong>. This list box is bound to a resource datasource. When the list box selection is changed, we initiate a custom <strong>FLTRES</strong><strong> </strong><a href="http://documentation.devexpress.com/#AspNet/CustomDocument5462">callback command</a> of the scheduler from the client side, as shown below:</p><p></p>

```js
scheduler.RaiseCallback('FLTRES|' + s.GetSelectedValues().join(','));
```

<p></p><p>We intercept and handle this command on the server side in the <strong>Page_Init</strong> event handler:</p><p></p>

```cs
   protected void Page_Init(object sender, EventArgs e) {
       string param = Page.Request["__CALLBACKPARAM"]; 
       if (!IsPostBack)
           Session["resourcesSelectCommand"] = defaultResourcesSelectCommand; 
       if (!string.IsNullOrEmpty(param) && param.Contains("FLTRES|")) {
           string resources = param.Substring(param.IndexOf('|') + 1);
           if (resources.Length > 0)
               Session["resourcesSelectCommand"] = string.Format("SELECT [ID], [Model] FROM [Cars] WHERE [ID] IN ({0})", resources);
           else
               Session["resourcesSelectCommand"] = defaultResourcesSelectCommand;
       } 
       SqlDataSourceResources.SelectCommand = Session["resourcesSelectCommand"].ToString();
   }
```

<p></p><p>That is, we parse callback command parameters and update the resource datasource SQL select command accordingly. We need to make this modification at such an early stage because it is too late to do this in the <strong>Page_Load</strong> or <a href="http://documentation.devexpress.com/#AspNet/DevExpressWebASPxSchedulerASPxScheduler_BeforeExecuteCallbackCommandtopic">ASPxScheduler.BeforeExecuteCallbackCommand</a> event handlers. This topic is described in the <a href="https://www.devexpress.com/Support/Center/p/B195177">Delete doesn't seem to fire events on server-side</a> thread in greater detail.</p><p></p><p>Prior to running this example, it is required to register a CarsXtraScheduling database in your local SQL Server instance. You can download the corresponding SQL scripts from the <a href="https://www.devexpress.com/Support/Center/p/E215">How to bind ASPxScheduler to MS SQL Server database</a> example.</p>

<br/>


