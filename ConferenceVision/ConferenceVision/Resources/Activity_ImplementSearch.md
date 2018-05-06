# Search

Adding search to a list in your mobile application is a common task. Let's take a look at how you might go about doing this on the `HomeView.xaml`.

## Results View

The `ListView` as it's currently implemented is nice for browser, but not ideal for seeing a set of search results in a concise manner. 

### DataTemplates

The item display for a `ListView` is handled by the `DataTemplate`. Before you [add a new template](https://docs.microsoft.com/en-us/xamarin/xamarin-forms/app-fundamentals/templates/data-templates/creating), refactor the existing code into a reusable template stored in your resources by [following this guide](https://docs.microsoft.com/en-us/xamarin/xamarin-forms/app-fundamentals/templates/data-templates/creating#creating-a-datatemplate-as-a-resource).

### Swap Templates 

Now when you enter search mode you need to change the `ListView` to use the new search template, and when in browse mode to use the default template.

For more information on DataTemplates, [check out this sample](https://developer.xamarin.com/samples/xamarin-forms/templates/datatemplates/).

### Filtering the Data

Next you want to actually get filtered results. This is a perfect job for Linq. The `ListView` is bound to `ObservableCollection<Memory> Memories` on the `HomeViewModel`. 

In the getter for that collection, implement a check to see if the user has typed anything in the search field. In order to do that, you'll also need to implement a new property for the search string and bind that to the search `Entry`. If you have text, return the filtered version of the collection, otherwise return the entire collection.

## Give it a try!

There are other ways to implement this for sure. Feel free to implement it as best suites the needs of your application.