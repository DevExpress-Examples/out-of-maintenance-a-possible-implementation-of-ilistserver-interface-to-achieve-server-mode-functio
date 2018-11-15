<!-- default file list -->
*Files to look at*:

* [DataObject.cs](./CS/WebSite/App_Code/DataObject.cs) (VB: [DataObject.vb](./VB/WebSite/App_Code/DataObject.vb))
* [Default.aspx](./CS/WebSite/Default.aspx) (VB: [Default.aspx.vb](./VB/WebSite/Default.aspx.vb))
* [Default.aspx.cs](./CS/WebSite/Default.aspx.cs) (VB: [Default.aspx.vb](./VB/WebSite/Default.aspx.vb))
<!-- default file list end -->
# A possible implementation of IListServer interface to achieve Server Mode functionality

### Disclaimer
This example is based on the undocumented internal API and targets advanced developers who are experts in the data access technology that will be used to retrieve data. In addition, implementing a custom Server Mode collection requires researching the source code of existing Server Mode components and our Data Grid. Refer to [Documentation for the IListServer and IAsyncListServer interfaces][1] for additional information.

### Description

The main aim of the sample is to demonstrate one of the possible ways to implement Server Mode by implementing the IListServer interface. It demonstrates which values each method should return. LINQ is used there only for demonstration purposes. In real-life scenarios, LINQ should be replaced with appropriate data access API.

Because of changes in IListServer implementation, this example has different solutions for different DevExpress versions.  

**See also:**  
[Bind a grid to a ObjectDataSource with EnablePaging][2]  
[Documentation for the IListServer and IAsyncListServer interfaces][1]

[1]: https://www.devexpress.com/Support/Center/Question/Details/S19875/documentation-for-the-ilistserver-and-iasynclistserver-interfaces
[2]: https://github.com/DevExpress-Examples/how-to-bind-aspxgridview-to-an-objectdatasource-with-enablepaging-e2672
