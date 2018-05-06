# 5 Minute Start: Exploring Xamarin.Forms 3.0

Let's get a quick taste of building beautiful mobile apps with Xamarin.Forms 3.0. In this quick walkthrough you will:

* Personalize the app
* Extend the app with cross-platform features
* Use FlexLayout and CSS to create a fluid layout
* Deploy to your own device!

> If you're on Windows you can use the new Live Reload (Preview) to see your XAML changes without leaving your debug session. Simply make changes to your XAML and save the file. 


## Personalize the App

The My Profile page `UserProfileView.xaml` is looking quite plain. Let's add your own personal touch.

1. Locate and open the `UserProfileView.xaml` within the .NET Standard Library project ConferenceVision > Views.
2. Remove the overlay content by finding the `<!-- REMOVE ME -->` comment.
3. Run the project to see what you're working with.


### Step  1: Add A StyleSheet

We have a CSS file ready for you to make this immediately a little better. To connect the CSS file, add a StyleSheet reference to the XAML.

```
<ContentPage.Resources>
    <StyleSheet Source="../Styles/UserProfileView.css"/>
</ContentPage.Resources>
```

Save the XAML and take another look. It's a little better, right?


### Step 2: Add an Image

A nice header background image will make this look a lot better too. Add this snippet to layout an image at the top where you see `<!-- TODO Header Image -->`.

```
<Image 
 Source="bg_seattle"
 AbsoluteLayout.LayoutBounds="0,0,1,340"
 AbsoluteLayout.LayoutFlags="WidthProportional"
 Aspect="AspectFill" />
```

Save it again. Nice! Want another image? We have a few options for you in the `Solution Items > Photos`. iOS and Android require keeping images in different folders, and they both use unique strategies for delivering images for different screen densities.

* **iOS:** ConferenceVision.iOS > Resources
  * @2x and @3x for example indicate higher densities
* **Android:** ConferenceVision.Android > Resources drawable-[density suffix]
  * hdpi, xhdpi, etc are folder suffixes indicating higher densities
  * USE the drawable-nodpi

Choose your photo and place it in the appropriate folders.
Just take the name of the image ignoring the path, suffixes, and file extension. Update the image's `Source` property and save it.

Good job!

## Step 3: Conent and Style

The bio is looking a little thin, why not add some details about yourself. Locate the `<!-- TODO Update Bio -->` and add a few sentences about yourself.

> Hint: make it a few lines so we can see how FlexLayout and CSS work here. :)

Now it probably looks something like this:

[image]

First, let's wire up how the `FlexLayout` container wrapping the bio `Label` and content is doing its job. Add a CSS class so we can style it.

```
<FlexLayout class="bio-container">
```

Over in the `UserProfileView.css` file that you referenced earlier, take a look at the `.container` block.

```
.bio-container {
    flex-direction: row;
    flex-wrap: wrap;
    justify-content: space-between;
    margin: 15,10;
    background-color: white;
}
```

The properties are pretty self explanatory here. The interesting one is `justify-content` which offers several options to distribute the child items in the layout. Here we're using `space-between` so we can add photos next.

> Note: Live Reload (Preview) doesn't yet reload the CSS file. We expect that to be implemented very soon.

All the content in the `FlexLayout` is collapsed on itself, but we want the labels and separator line (the `BoxView`) to be on top of each other vertically. We could change the `FlexLayout` to use `flex-direction: column` but that won't work for our grid of images that come next. Instead, let's use `FlexLayout.Basis` on our child content so they take up 100% of the view width and wrap the next item.

Add `FlexLayout.Basis="100%"` in the XAML to the labels and box view.

## Step 4: Image Grid

Notice the `Image` controls are already spacing out, but you didn't add a `class=""` tag to them. That's because the CSS is already styling all images that are children of the `FlexLayout` with this block of CSS:

```
.bio-container > image {
    flex-basis: 45%;
    height: 80;
    margin: 0,10;
}
```

You don't want to see colorful boxes, but rather your own content, so let's wire that up quickly. 

1. Stop debugging.
2. Delete the list of images in the XAML.  
3. In `UserProfileView.xaml.cs` uncomment the code in `OnAppearing`.

Now when the page loads we will display a grid of photos you've taken with the app. 
 
## Final Step: Deploy to Device
Let's wrap this up and get your app on your own device. 

> **Debug vs Release**
> 
> In order to generate a release build you need to have code signing certificates and provisioning profiles. That's a little more than you have time for right now, so this means:
> - You will be running a **Debug** build
> - It will run slower
> - You should test a **Release** as soon as you can to really see the speed of Xamarin

### For iOS

- Select `MOVIA.iOS` or `ConferenceVision.Android` as your startup project.
    - For iPhone devices select `Debug|iPhone` as your target
    - For Android devices select `Debug` as your target
- Connect your phone with a cable to the computer
- Hit debug

The app will be built now for a physical device and deployed to your own phone.

# Congratulations! You're done.

