# Creating Custom Page-Aware Components in Scryber

Based on analysis of the Scryber v5.0.6-Package-Release codebase, this document explains how page headers work and how to create custom components that render dynamic data per page within headers.

## How Scryber Page Headers Work

**Key Files Analyzed:**
- `Scryber.Components/Components/PageAdornment.cs:48-56` - PDFPageHeader definition
- `Scryber.Components/Components/PageBase.cs:82-97` - Header property and management
- `Scryber.Components/Components/PageNumberLabel.cs:28-50` - Example page-aware component
- `Scryber.Components/Components/PageOfLabel.cs:28-50` - Another page-aware component

## The Pattern for Page-Aware Components

Scryber achieves dynamic per-page content using a **text proxy pattern**:

1. **Initial Layout Phase**: Components create placeholder text with estimated content
2. **Layout Complete Phase**: After layout is done, the actual page-specific data is known and the proxy text is updated
3. **Rendering**: The final correct content is rendered

## How to Create Your Custom Page-Aware Component

Here's how to create a custom component similar to `PageNumberLabel` that can access dynamic data per page:

### 1. Extend TextBase and Use the Proxy Pattern

```csharp
[PDFParsableComponent("CustomPageData")]
public class CustomPageDataLabel : TextBase
{
    // Your custom properties
    [PDFAttribute("data-source")]
    public string DataSource { get; set; }
    
    // Reference to layout document and current page
    private Layout.PDFLayoutDocument _doc;
    private int _renderpageindex = -1;
    private Style _fullstyle = null;
    
    // The text proxy that will be updated with actual data
    private Scryber.Text.PDFTextProxyOp _dataProxy;
    
    protected override string BaseText
    {
        get { return this.GetDisplayText(false); }
        set { throw new InvalidOperationException("Cannot set base text"); }
    }
    
    public CustomPageDataLabel() : base(PDFObjectTypes.Text) { }
```

### 2. Override CreateReader for Initial Layout

```csharp
protected override Text.PDFTextReader CreateReader(PDFLayoutContext context, Style style)
{
    _doc = context.DocumentLayout;
    _renderpageindex = _doc.CurrentPageIndex;
    _fullstyle = style;
    
    // Create placeholder text for initial layout
    string placeholderText = this.GetDisplayText(false);
    
    // Create the proxy that will be updated later
    var proxyOp = new Text.PDFTextProxyOp(this, "CustomPageData", placeholderText);
    var arrayReader = new Text.PDFArrayTextReader(new Text.PDFTextOp[] { proxyOp });
    
    _dataProxy = proxyOp;
    return arrayReader;
}
```

### 3. Override RegisterLayoutComplete for Final Data

```csharp
internal override void RegisterLayoutComplete(PDFLayoutContext context)
{
    base.RegisterLayoutComplete(context);
    
    // Get the actual page index where this component was placed
    PDFComponentArrangement arrange = this.GetFirstArrangement();
    if (arrange != null)
    {
        this._renderpageindex = arrange.PageIndex;
        this._fullstyle = arrange.FullStyle;
    }
    
    // Now update the proxy with the real data
    if (_dataProxy != null)
    {
        string actualText = this.GetDisplayText(true);
        _dataProxy.Text = actualText;
    }
}
```

### 4. Implement Your Data Logic

```csharp
private string GetDisplayText(bool rendering)
{
    if (!rendering)
        return "Loading..."; // Placeholder during layout
    
    // Access page-specific data here
    // You have access to:
    // - _renderpageindex: The actual page index
    // - _doc: The layout document
    // - this.Document: The source document
    // - Context data through the layout context
    
    return GetCustomDataForPage(_renderpageindex);
}

private string GetCustomDataForPage(int pageIndex)
{
    // Your custom logic here
    // Examples:
    // - Access document.Items collection
    // - Look up data by page index
    // - Calculate values based on page content
    // - Access parent page properties
    
    return $"Page {pageIndex + 1} Custom Data";
}
```

## Key Insights from the Architecture

1. **Registration**: Components are registered using `[PDFParsableComponent("YourTagName")]`
2. **Page Context**: Access current page via `context.DocumentLayout.CurrentPageIndex` 
3. **Two-Phase Rendering**: Initial layout uses estimates, final rendering uses actual data
4. **Text Proxy Pattern**: `PDFTextProxyOp` allows updating text after layout completion
5. **Component Arrangement**: `GetFirstArrangement()` gives you the final page placement

## Advanced Capabilities

You can also:
- Access the page's data context via `context.Items`
- Look up other components using `Document.FindAComponentById()`
- Access page-specific styles and properties
- Implement complex formatting like the page number components do

This pattern allows you to create any component that needs to render different content based on which page it appears on, giving you the same flexibility as the built-in page number functionality but for your own custom data.

## Data Context - What's Available?

**The data context includes BOTH:**

1. **Page-specific data**: Data that was bound specifically to the current page during layout
2. **Document-wide model**: The original model/data that was applied to the entire document template

**Key Discovery from `PageBase.cs:734-808`:**
- Pages can have their own `Params` collection (`PageBase.cs:62-70`)  
- During data binding, pages **merge** their local params with the document-wide context
- This creates a **layered data system**: page params override document params with the same keys

## Passing Arguments via HTML

**Yes, absolutely!** Components can receive parameters through HTML attributes using `[PDFAttribute]`:

```csharp
[PDFParsableComponent("CustomPageData")]
public class CustomPageDataLabel : TextBase
{
    // Simple string parameter
    [PDFAttribute("data-source")]
    public string DataSource { get; set; }
    
    // You can also use namespaced attributes
    [PDFAttribute("custom-format", Style.PDFStylesNamespace)] 
    public string CustomFormat { get; set; }
    
    // Different data types work too
    [PDFAttribute("max-length")]
    public int MaxLength { get; set; }
    
    [PDFAttribute("show-prefix")]
    public bool ShowPrefix { get; set; }
}
```

**Usage in HTML:**
```html
<pdf:PageHeader>
    <!-- Simple usage -->
    <CustomPageData data-source="customerName" />
    
    <!-- With multiple parameters -->
    <CustomPageData 
        data-source="orderTotal" 
        custom-format="Currency: {0:C}" 
        max-length="50" 
        show-prefix="true" />
</pdf:PageHeader>
```

## Accessing Both Data Sources

```csharp
private string GetCustomDataForPage(int pageIndex)
{
    // Access parameters passed via HTML
    string dataKey = this.DataSource; // From [PDFAttribute("data-source")]
    string format = this.CustomFormat; // From [PDFAttribute("custom-format")]
    
    // Access the current data context (during CreateReader)
    var contextData = _layoutContext?.Items;
    
    // Look up specific data by key
    if (contextData?.Contains(dataKey) == true)
    {
        var value = contextData[dataKey];
        if (!string.IsNullOrEmpty(format))
            return string.Format(format, value);
        else
            return value?.ToString() ?? "";
    }
    
    // Access current data object from binding stack
    if (_layoutContext?.DataStack?.HasData == true)
    {
        var currentDataObject = _layoutContext.DataStack.Current;
        // Use reflection or dynamic to access properties
    }
    
    return "No data found";
}
```

## Advanced Data Access Patterns

```csharp
protected override Text.PDFTextReader CreateReader(PDFLayoutContext context, Style style)
{
    _doc = context.DocumentLayout;
    _layoutContext = context; // Store for later use
    _renderpageindex = _doc.CurrentPageIndex;
    
    // You have access to:
    // 1. context.Items - The current item collection (page + document data merged)
    // 2. context.DataStack - The current binding stack (nested data contexts)  
    // 3. this.DataSource - HTML attribute parameters
    // 4. Page-specific data through the page's Params collection
    
    // Example: Look up page-specific configuration
    if (context.Items.Contains("pageConfig"))
    {
        var pageConfig = context.Items["pageConfig"];
        // Use page config to modify behavior
    }
    
    // Rest of CreateReader implementation...
}
```

## Complete Example: Advanced Custom Component

Here's a comprehensive example showing all the capabilities:

```csharp
[PDFParsableComponent("CustomerInfo")]
public class CustomerInfoLabel : TextBase
{
    // HTML parameters for configuration
    [PDFAttribute("data-field")]
    public string DataField { get; set; } = "customerName";
    
    [PDFAttribute("format-template")]
    public string FormatTemplate { get; set; } = "{0}";
    
    [PDFAttribute("page-prefix")]
    public string PagePrefix { get; set; } = "";
    
    [PDFAttribute("max-length")]
    public int MaxLength { get; set; } = 100;
    
    [PDFAttribute("fallback-text")]
    public string FallbackText { get; set; } = "N/A";
    
    // Internal state
    private Layout.PDFLayoutDocument _doc;
    private PDFLayoutContext _layoutContext;
    private int _renderpageindex = -1;
    private Style _fullstyle = null;
    private Scryber.Text.PDFTextProxyOp _dataProxy;
    
    public CustomerInfoLabel() : base(PDFObjectTypes.Text) { }
    
    protected override string BaseText
    {
        get { return this.GetDisplayText(false); }
        set { throw new InvalidOperationException("Cannot set base text"); }
    }
    
    protected override Text.PDFTextReader CreateReader(PDFLayoutContext context, Style style)
    {
        _doc = context.DocumentLayout;
        _layoutContext = context;
        _renderpageindex = _doc.CurrentPageIndex;
        _fullstyle = style;
        
        // Create placeholder for initial layout
        string placeholderText = this.GetDisplayText(false);
        
        var proxyOp = new Text.PDFTextProxyOp(this, "CustomerInfo", placeholderText);
        var arrayReader = new Text.PDFArrayTextReader(new Text.PDFTextOp[] { proxyOp });
        
        _dataProxy = proxyOp;
        return arrayReader;
    }
    
    internal override void RegisterLayoutComplete(PDFLayoutContext context)
    {
        base.RegisterLayoutComplete(context);
        
        // Get actual page placement
        PDFComponentArrangement arrange = this.GetFirstArrangement();
        if (arrange != null)
        {
            this._renderpageindex = arrange.PageIndex;
            this._fullstyle = arrange.FullStyle;
        }
        
        // Update with real data
        if (_dataProxy != null)
        {
            string actualText = this.GetDisplayText(true);
            _dataProxy.Text = actualText;
        }
    }
    
    private string GetDisplayText(bool rendering)
    {
        if (!rendering)
            return "Loading " + DataField + "..."; // Placeholder
        
        try
        {
            return GetDataForPage(_renderpageindex);
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }
    }
    
    private string GetDataForPage(int pageIndex)
    {
        string result = FallbackText;
        
        // 1. Try to get data from context using the specified field
        if (_layoutContext?.Items?.Contains(DataField) == true)
        {
            var value = _layoutContext.Items[DataField];
            if (value != null)
            {
                result = value.ToString();
            }
        }
        // 2. Try to get from current data stack object
        else if (_layoutContext?.DataStack?.HasData == true)
        {
            var currentData = _layoutContext.DataStack.Current;
            if (currentData != null)
            {
                // Use reflection to get property value
                var property = currentData.GetType().GetProperty(DataField);
                if (property != null)
                {
                    var value = property.GetValue(currentData);
                    if (value != null)
                    {
                        result = value.ToString();
                    }
                }
                // Or try dynamic access for anonymous objects
                else if (currentData is System.Dynamic.ExpandoObject)
                {
                    var dict = currentData as IDictionary<string, object>;
                    if (dict.ContainsKey(DataField))
                    {
                        result = dict[DataField]?.ToString();
                    }
                }
            }
        }
        
        // 3. Apply formatting and constraints
        if (!string.IsNullOrEmpty(result))
        {
            // Apply format template
            if (!string.IsNullOrEmpty(FormatTemplate) && FormatTemplate != "{0}")
            {
                try
                {
                    result = string.Format(FormatTemplate, result);
                }
                catch
                {
                    // Fall back to original if formatting fails
                }
            }
            
            // Add page-specific prefix
            if (!string.IsNullOrEmpty(PagePrefix))
            {
                result = $"{PagePrefix} (Page {pageIndex + 1}): {result}";
            }
            
            // Apply length constraint
            if (MaxLength > 0 && result.Length > MaxLength)
            {
                result = result.Substring(0, MaxLength - 3) + "...";
            }
        }
        
        return result;
    }
}
```

## Usage Examples

### Basic Usage (HTML Format)
```html
<header>
    <!-- Simple data field -->
    <CustomerInfo data-field="customerName" />
</header>
```

### Advanced Configuration (HTML Format)
```html
<header>
    <!-- Standard page number -->
    <page />
    
    <!-- Formatted customer info with page prefix -->
    <CustomerInfo 
        data-field="customerName" 
        format-template="Customer: {0}"
        page-prefix="Report"
        max-length="50"
        fallback-text="No Customer" />
    
    <!-- Order total with currency formatting -->
    <CustomerInfo 
        data-field="orderTotal" 
        format-template="Total: {0:C}"
        page-prefix="Invoice" />
</header>

<footer>
    <!-- Standard footer with page numbers -->
    <p>Page <page /> of <page property="Total" /></p>
</footer>
```

### XML Format Alternative
```xml
<doc:Document xmlns:doc='http://www.scryber.co.uk/schemas/core/release/v1/Scryber.Components.xsd'>
    <Pages>
        <doc:Page>
            <Header>
                <CustomerInfo data-field="customerName" format-template="Customer: {0}" />
            </Header>
            <Content>
                <!-- Page content -->
            </Content>
        </doc:Page>
    </Pages>
</doc:Document>
```

## Key Benefits of This Approach

1. **Reusable**: Same component, different configurations via HTML attributes
2. **Page-Aware**: Automatically gets the right data for each page
3. **Flexible Data Access**: Works with named properties, Items collection, or data stack
4. **Error-Resistant**: Graceful fallbacks and error handling
5. **Formatting Options**: Built-in formatting and length constraints
6. **Performance**: Two-phase rendering optimizes layout performance

## Integration Points

- **Document Model**: Access document-wide data via `context.Items`
- **Page Context**: Page-specific data automatically merged
- **Data Binding**: Works with Scryber's data binding system
- **HTML Templates**: Configurable directly in HTML markup
- **Layout System**: Integrates seamlessly with Scryber's layout engine

This pattern gives you the same power as built-in components like `PageNumber` but for your own custom business data!