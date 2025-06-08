#if ANDROID
using Android.Content;
using Android.Provider;
using Android.OS;
using Microsoft.Maui.ApplicationModel;
using System.Text;
using System.Threading.Tasks;

public static class FileWorker
{
    public static async Task ExportCsvToDocumentsAndShareAsync(string fileName, string csvContent)
    {
        var data = Encoding.UTF8.GetBytes(csvContent);
        var mimeType = "text/csv";

        var values = new ContentValues();
        values.Put(MediaStore.IMediaColumns.DisplayName, fileName);
        values.Put(MediaStore.IMediaColumns.MimeType, mimeType);
        values.Put(MediaStore.IMediaColumns.RelativePath, Android.OS.Environment.DirectoryDocuments);

        var uri = Android.App.Application.Context.ContentResolver.Insert(
            MediaStore.Files.GetContentUri("external"), values);

        using var output = Android.App.Application.Context.ContentResolver.OpenOutputStream(uri);
        await output.WriteAsync(data, 0, data.Length);

        var filePath = Path.Combine(FileSystem.CacheDirectory, fileName);
        File.WriteAllText(filePath, csvContent, Encoding.UTF8);

        await Share.Default.RequestAsync(new ShareFileRequest
        {
            Title = "Поделиться CSV",
            File = new ShareFile(filePath)
        });
    }
}
#endif