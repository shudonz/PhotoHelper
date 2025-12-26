# PhotoHelper

A Windows desktop application for batch updating photo metadata (EXIF, GPS, date, description, and keywords) for multiple photos at once.

## Overview

PhotoHelper is a WPF application built with .NET 9.0 that simplifies the process of updating metadata for multiple photos simultaneously. It's particularly useful when you have a collection of photos from a specific location and date that need consistent metadata applied.

## Features

- **Batch Processing**: Update metadata for all photos in a folder at once
- **GPS/Location Data**: Convert addresses to GPS coordinates using geocoding and embed them into photo EXIF data
- **Date Management**: Set the date/time for all photos
- **Description & Keywords**: Add custom descriptions and keywords to your photos
- **Progress Tracking**: Real-time progress bar and status updates during processing
- **Backup Safety**: Automatically creates backups before modifying photos
- **Supported Formats**: Works with JPG, JPEG, PNG, TIFF, and TIF files

## Requirements

- **Operating System**: Windows (with .NET 9.0 support)
- **.NET Runtime**: .NET 9.0 Desktop Runtime (Windows)
- **Architecture**: Windows x64 or x86

## Dependencies

The application uses the following NuGet packages:
- **ExifLibNet** (v2.1.4) - For reading and writing EXIF metadata
- **MetadataExtractor** (v2.9.0) - For extracting metadata from images
- **XmpCore** (v6.1.10.1) - For XMP metadata support

## Installation

### Option 1: Build from Source

1. **Prerequisites**:
   - Install [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
   - Windows 10/11

2. **Clone the repository**:
   ```bash
   git clone https://github.com/shudonz/PhotoHelper.git
   cd PhotoHelper
   ```

3. **Build the project**:
   ```bash
   dotnet build
   ```

4. **Run the application**:
   ```bash
   dotnet run --project PhotoHelper/PhotoHelper.csproj
   ```

### Option 2: Publish as Standalone

To create a standalone executable:

```bash
dotnet publish PhotoHelper/PhotoHelper.csproj -c Release -r win-x64 --self-contained
```

The executable will be in `PhotoHelper/bin/Release/net9.0-windows/win-x64/publish/`

## Usage

### Step-by-Step Guide

1. **Select Photo Folder**
   - Click the "Browse..." button
   - Navigate to the folder containing your photos
   - Select the folder

2. **Enter Location Address**
   - Type the address where the photos were taken
   - Example: "1600 Amphitheatre Parkway, Mountain View, CA"
   - The application will automatically geocode this to GPS coordinates

3. **Select Photo Date**
   - Choose the date when the photos were taken using the date picker

4. **Add Additional Metadata (Optional)**
   - **Description**: Enter a description for the photos
   - **Keywords**: Enter comma-separated keywords (e.g., "vacation, travel, family")

5. **Apply Metadata**
   - Click "Apply Metadata to All Photos"
   - Monitor the progress bar and status log
   - Wait for the completion message

### What Gets Updated

The application updates the following EXIF metadata fields:
- **Date/Time**: DateTimeOriginal, DateTime, DateTimeDigitized
- **GPS Coordinates**: GPSLatitude, GPSLongitude, GPSLatitudeRef, GPSLongitudeRef
- **Description**: ImageDescription, UserComment
- **Keywords**: Stored in the Artist field

## Geocoding

PhotoHelper uses the [Nominatim OpenStreetMap API](https://nominatim.openstreetmap.org/) for free geocoding without requiring an API key. Please use it responsibly:
- Limit the number of requests
- Provide accurate addresses
- Follow [Nominatim usage policy](https://operations.osmfoundation.org/policies/nominatim/)

## Safety Features

- **Automatic Backups**: Creates `.backup` files before modifying originals
- **Error Handling**: Gracefully handles errors and restores from backup if needed
- **Validation**: Validates all inputs before processing
- **Progress Feedback**: Shows detailed status for each photo processed

## Technical Details

### Project Structure

```
PhotoHelper/
├── PhotoHelper/
│   ├── App.xaml                    # WPF application definition
│   ├── MainWindow.xaml             # Main UI layout
│   ├── MainWindow.xaml.cs          # UI logic and event handlers
│   ├── PhotoMetadataWriter.cs      # EXIF metadata writing logic
│   ├── GeocodingService.cs         # Address to GPS conversion
│   ├── PhotoHelper.csproj          # Project configuration
│   └── AssemblyInfo.cs             # Assembly metadata
├── README.md
└── .gitignore
```

### Architecture

- **UI Framework**: WPF (Windows Presentation Foundation)
- **Language**: C# with .NET 9.0
- **Async/Await**: Used for geocoding and batch processing to keep UI responsive
- **EXIF Library**: ExifLibNet for metadata manipulation

## Troubleshooting

### Common Issues

**Issue**: "Failed to geocode the address"
- **Solution**: Check your internet connection and verify the address is correct

**Issue**: "No photo files found"
- **Solution**: Ensure the folder contains supported image formats (JPG, JPEG, PNG, TIFF, TIF)

**Issue**: Application won't start
- **Solution**: Ensure .NET 9.0 Desktop Runtime is installed

**Issue**: "Failed to update metadata"
- **Solution**: Check that the photo files are not read-only and you have write permissions

## Contributing

Contributions are welcome! Please feel free to submit pull requests or open issues for bugs and feature requests.

## License

This project is open source. Please check the repository for license details.

## Acknowledgments

- Built with .NET 9.0 and WPF
- Uses OpenStreetMap Nominatim for geocoding
- EXIF handling powered by ExifLibNet

## Contact

For questions or support, please open an issue on the GitHub repository.

---

**Note**: This application modifies photo files directly. While it creates backups, it's recommended to keep your own backup of important photos before batch processing.