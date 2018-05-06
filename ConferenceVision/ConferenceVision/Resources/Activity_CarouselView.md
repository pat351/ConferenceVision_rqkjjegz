# Adding a CarouselView 

This is a popular control, especially for flipping through a set of images like we have on our home view. Why not give this a look and replace the default ListView.

## Add the CarouselView NuGet

For this sample we recommend using the very popular CarouselView here https://github.com/alexrainman/CarouselView. 

## Initialize the control

### AppDelegate.cs and MainActivity.cs

Just after `Xamarin.Forms.Init()` add:

```
CarouselViewRenderer.Init();
```

### UWP Setup

Where you initialize Xamarin.Forms, use this code to bring in the `CarouselView` assembly:
``
List<Assembly> assembliesToInclude = new List<Assembly>();
assembliesToInclude.Add(typeof(CarouselViewRenderer).GetTypeInfo().Assembly);
Xamarin.Forms.Forms.Init(e, assembliesToInclude);
```

## Implement the XAML

Now on `HomeView.xaml` let's add the necessary markup. First, add the required `xmlns`:

```
xmlns:carousel="clr-namespace:CarouselView.FormsPlugin.Abstractions;assembly=CarouselView.FormsPlugin.Abstractions"
```

Then in place of the `ListView` add the `CarouselView`:


```
<carousel:CarouselViewControl 
            AbsoluteLayout.LayoutBounds="0, 0, 1, 1"
            AbsoluteLayout.LayoutFlags="SizeProportional"
            Orientation="Horizontal" ShowIndicators="true"  
            InterPageSpacing="-10" 
            ItemsSource="{Binding Memories}">
            <carousel:CarouselViewControl.ItemTemplate>
                <DataTemplate>
                    <StackLayout Padding="20"
                         HorizontalOptions="FillAndExpand">
                        <ffimageloading:CachedImage
                           WidthRequest="300"
                            HeightRequest="300"
                            RetryCount="0"
                            
                            Source="{Binding MediaPath, Converter={StaticResource ImageSourceConverter}}"     
                            Aspect="AspectFill">
                            <ffimageloading:CachedImage.LoadingPlaceholder>
                                <OnPlatform x:TypeArguments="ImageSource">
                                    <On Platform="iOS, Android" Value="placeholder" />
                                    <On Platform="UWP, WinRT, WinPhone" Value="Assets/placeholder.png" />
                                </OnPlatform>
                            </ffimageloading:CachedImage.LoadingPlaceholder>
                            <ffimageloading:CachedImage.ErrorPlaceholder>
                                <OnPlatform x:TypeArguments="ImageSource">
                                    <On Platform="iOS, Android" Value="noimage" />
                                    <On Platform="UWP, WinRT, WinPhone" Value="Assets/noimage.png" />
                                </OnPlatform>
                            </ffimageloading:CachedImage.ErrorPlaceholder>
                        </ffimageloading:CachedImage>
                    </StackLayout>
                </DataTemplate>
            </carousel:CarouselViewControl.ItemTemplate>
        </carousel:CarouselViewControl>
```

Save and run!

## What's Next?
Explore the other features of the `CarouselView` and customize the DataTemplate to your liking.
