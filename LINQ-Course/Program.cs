// See https://aka.ms/new-console-template for more information

string path = @"C:\windows";
ShowLargeFileWithOutLinq(path);
Console.WriteLine("**********");
ShowLargeFileWithLinq(path);

static void ShowLargeFileWithLinq(string path)
{
    var query = from file in new DirectoryInfo(path).GetFiles()
                orderby file.Length descending
                select file;

    var query2 = new DirectoryInfo(path).GetFiles()
                    .OrderByDescending(f => f.Length)
                    .Take(5);

    foreach (var file in query.Take(5))
    {
        Console.WriteLine($"{file.Name,-20} : {file.Length,10:N0}");    
    }
}

static void ShowLargeFileWithOutLinq(string path)
{
    DirectoryInfo directory = new DirectoryInfo(path);
    FileInfo[] files = directory.GetFiles();
    Array.Sort(files, new FileInfoComparer());

    for (int i = 0; i < 5; i++)
    {
        FileInfo file = files[i];
        Console.WriteLine($"{file.Name,-20} : {file.Length,10:N0}");
    }
}

public class FileInfoComparer : IComparer<FileInfo>
{
    public int Compare(FileInfo x, FileInfo y)
    {
        return y.Length.CompareTo(x.Length);
    }
}