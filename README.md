# wingman
Website Instructed Graphical Manipulator

This library uses the excellent ImageProcessor NuGet package (https://www.nuget.org/packages/ImageProcessor/) to manipulate images through a website URL.

- Crop Images to a Square
- Resize based on width maintaining aspect ratio
- Optimize image quality
- Reformat the image to webp, jpeg, gif, or png

### Usage

Configure just three web.config values to get it working out the box:

*MaximumImageDimension* - integer value representing largest width/height of an image
*SourceDirectory* - absolute path to the source of all your images
*DerivedDirectory* - a directory where all resized images will be stored (must have write permissions)

Once configured, you can use the default manipulation URLs as seen in the RouteConfig.

### Advanced Usage (recommended)

You can create your own custom routes that map to the examples through the wingmanCustomRoutes web configuration section. This allows you to have more control over the site along with leading to simpler URLs.
