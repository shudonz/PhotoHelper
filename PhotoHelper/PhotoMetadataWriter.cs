using System.IO;
using ExifLibrary;

namespace PhotoHelper;

public class PhotoMetadataWriter
{
    public void UpdateMetadata(
        string filePath,
        DateTime date,
        double latitude,
        double longitude,
        string? description,
        string? keywords)
    {
        try
        {
            // Create a backup of the original file
            var backupPath = filePath + ".backup";
            if (!File.Exists(backupPath))
            {
                File.Copy(filePath, backupPath, true);
            }

            // Read the image file
            var imageFile = ImageFile.FromFile(filePath);

            // Set DateTimeOriginal (EXIF tag 0x9003)
            imageFile.Properties.Set(ExifTag.DateTimeOriginal, date);
            imageFile.Properties.Set(ExifTag.DateTime, date);
            imageFile.Properties.Set(ExifTag.DateTimeDigitized, date);

            // Set GPS Latitude
            var latRef = latitude >= 0 ? GPSLatitudeRef.North : GPSLatitudeRef.South;
            imageFile.Properties.Set(ExifTag.GPSLatitudeRef, latRef);
            
            // Convert latitude to degrees, minutes, seconds
            var latDMS = ConvertToDMS(Math.Abs(latitude));
            imageFile.Properties.Set(ExifTag.GPSLatitude, latDMS);

            // Set GPS Longitude
            var lonRef = longitude >= 0 ? GPSLongitudeRef.East : GPSLongitudeRef.West;
            imageFile.Properties.Set(ExifTag.GPSLongitudeRef, lonRef);
            
            // Convert longitude to degrees, minutes, seconds
            var lonDMS = ConvertToDMS(Math.Abs(longitude));
            imageFile.Properties.Set(ExifTag.GPSLongitude, lonDMS);

            // Set Description (EXIF ImageDescription and XMP Description)
            if (!string.IsNullOrWhiteSpace(description))
            {
                imageFile.Properties.Set(ExifTag.ImageDescription, description);
                imageFile.Properties.Set(ExifTag.UserComment, description);
            }

            // Set Keywords - using UserComment for keywords
            if (!string.IsNullOrWhiteSpace(keywords))
            {
                // Store keywords in Artist or Copyright field as fallback
                imageFile.Properties.Set(ExifTag.Artist, keywords);
            }

            // Save the file
            imageFile.Save(filePath);

            // Delete backup if successful
            if (File.Exists(backupPath))
            {
                File.Delete(backupPath);
            }
        }
        catch (Exception ex)
        {
            // Restore from backup if something went wrong
            var backupPath = filePath + ".backup";
            if (File.Exists(backupPath))
            {
                File.Copy(backupPath, filePath, true);
                File.Delete(backupPath);
            }
            throw new Exception($"Failed to update metadata: {ex.Message}", ex);
        }
    }

    private object ConvertToDMS(double coordinate)
    {
        // Convert decimal degrees to degrees, minutes, seconds
        var degrees = (uint)Math.Floor(coordinate);
        var remainder = (coordinate - degrees) * 60;
        var minutes = (uint)Math.Floor(remainder);
        var seconds = (remainder - minutes) * 60;

        // GPS coordinates in ExifLibrary need to be arrays of rationals
        // Using tuples as rational numbers (numerator, denominator)
        return new float[] { degrees, minutes, (float)seconds };
    }
}
