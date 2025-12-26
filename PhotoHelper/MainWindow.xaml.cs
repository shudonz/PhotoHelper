using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace PhotoHelper;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private string? _selectedFolderPath;

    public MainWindow()
    {
        InitializeComponent();
        PhotoDatePicker.SelectedDate = DateTime.Today;
    }

    private void BrowseFolderButton_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFolderDialog
        {
            Title = "Select Photo Folder"
        };

        if (dialog.ShowDialog() == true)
        {
            _selectedFolderPath = dialog.FolderName;
            FolderPathTextBox.Text = _selectedFolderPath;
            UpdateStatus($"Selected folder: {_selectedFolderPath}");
        }
    }

    private async void ApplyButton_Click(object sender, RoutedEventArgs e)
    {
        // Validate inputs
        if (string.IsNullOrEmpty(_selectedFolderPath))
        {
            MessageBox.Show("Please select a photo folder.", "Missing Information", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (!Directory.Exists(_selectedFolderPath))
        {
            MessageBox.Show("The selected folder does not exist.", "Error", 
                MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        if (string.IsNullOrWhiteSpace(AddressTextBox.Text) || 
            AddressTextBox.Text.Contains("e.g.,"))
        {
            MessageBox.Show("Please enter a valid address.", "Missing Information", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (!PhotoDatePicker.SelectedDate.HasValue)
        {
            MessageBox.Show("Please select a date.", "Missing Information", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        // Disable UI during processing
        ApplyButton.IsEnabled = false;
        BrowseFolderButton.IsEnabled = false;

        try
        {
            UpdateStatus("Starting metadata update process...");

            // Get GPS coordinates from address
            var geocoder = new GeocodingService();
            UpdateStatus($"Geocoding address: {AddressTextBox.Text}");
            var coordinates = await geocoder.GeocodeAddressAsync(AddressTextBox.Text);

            if (coordinates == null)
            {
                MessageBox.Show("Failed to geocode the address. Please check the address and try again.", 
                    "Geocoding Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            CoordinatesTextBlock.Text = $"GPS: {coordinates.Value.Latitude:F6}, {coordinates.Value.Longitude:F6}";
            UpdateStatus($"Geocoded to: Lat={coordinates.Value.Latitude:F6}, Lon={coordinates.Value.Longitude:F6}");

            // Get photo files
            var photoExtensions = new[] { ".jpg", ".jpeg", ".png", ".tiff", ".tif" };
            var photoFiles = Directory.GetFiles(_selectedFolderPath)
                .Where(f => photoExtensions.Contains(Path.GetExtension(f).ToLowerInvariant()))
                .ToArray();

            if (photoFiles.Length == 0)
            {
                MessageBox.Show("No photo files found in the selected folder.", "No Photos", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            UpdateStatus($"Found {photoFiles.Length} photo(s) to process");

            // Prepare metadata
            var date = PhotoDatePicker.SelectedDate.Value;
            var description = string.IsNullOrWhiteSpace(DescriptionTextBox.Text) 
                ? null : DescriptionTextBox.Text;
            var keywords = string.IsNullOrWhiteSpace(KeywordsTextBox.Text) || 
                          KeywordsTextBox.Text.Contains("vacation, travel, family")
                ? null : KeywordsTextBox.Text;

            // Process each photo
            var metadataWriter = new PhotoMetadataWriter();
            ProgressBar.Maximum = photoFiles.Length;
            ProgressBar.Value = 0;

            int successCount = 0;
            int failCount = 0;

            foreach (var photoFile in photoFiles)
            {
                try
                {
                    UpdateStatus($"Processing: {Path.GetFileName(photoFile)}");
                    
                    await Task.Run(() =>
                    {
                        metadataWriter.UpdateMetadata(
                            photoFile,
                            date,
                            coordinates.Value.Latitude,
                            coordinates.Value.Longitude,
                            description,
                            keywords
                        );
                    });

                    successCount++;
                    UpdateStatus($"✓ Successfully updated: {Path.GetFileName(photoFile)}");
                }
                catch (Exception ex)
                {
                    failCount++;
                    UpdateStatus($"✗ Failed to update {Path.GetFileName(photoFile)}: {ex.Message}");
                }

                ProgressBar.Value++;
            }

            UpdateStatus($"\n--- COMPLETE ---");
            UpdateStatus($"Successfully updated: {successCount} photo(s)");
            if (failCount > 0)
            {
                UpdateStatus($"Failed: {failCount} photo(s)");
            }

            MessageBox.Show(
                $"Processing complete!\n\nSuccessfully updated: {successCount}\nFailed: {failCount}", 
                "Complete", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            UpdateStatus($"Error: {ex.Message}");
            MessageBox.Show($"An error occurred: {ex.Message}", "Error", 
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            // Re-enable UI
            ApplyButton.IsEnabled = true;
            BrowseFolderButton.IsEnabled = true;
        }
    }

    private void UpdateStatus(string message)
    {
        StatusTextBlock.Text += message + Environment.NewLine;
        
        // Auto-scroll to bottom
        if (StatusTextBlock.Parent is ScrollViewer scrollViewer)
        {
            scrollViewer.ScrollToBottom();
        }
    }
}