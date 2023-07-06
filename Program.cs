using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
#pragma warning disable CA1416

class RemoveWhiteBoundaries
{
    public unsafe static Bitmap TrimWhiteBoundaries(Bitmap SrcImg)
    {


        int Width = SrcImg.Width, Height = SrcImg.Height;

        int bottom = 0;
        int left = Width;
        int right = 0;
        int top = Height;


        BitmapData Data = SrcImg.LockBits(new Rectangle(0, 0, Width, Height),
         ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
        byte* Ptr = (byte*)Data.Scan0;
        int Stride = Data.Stride;

        for (int y = 0; y < Height; y++)
        {
            byte* Row = (byte*)Ptr + y * Stride;

            for (int x = 0; x < Width; x++)
            {
                if (Row[x] != 0xff)
                {
                    if (x < left)
                        left = x;

                    if (x >= right)
                        right = x + 1;

                    if (y < top)
                        top = y;

                    if (y >= bottom)
                        bottom = y + 1;
                }
            }
        }

        SrcImg.UnlockBits(Data);

        if (left < right && top < bottom)
            return SrcImg.Clone(new Rectangle(left, top, right - left, bottom - top),
             SrcImg.PixelFormat);
        else
        {
            Console.WriteLine("All of the image is White");
            return SrcImg;
        }
    }

    public void Run()
    {
        string dir = @"results";
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        Console.Write("Enter the Image path : ");
        string path = Console.ReadLine();
        Bitmap test = new Bitmap(path);

        var watch = new System.Diagnostics.Stopwatch();
        watch.Start();

        test = TrimWhiteBoundaries(test);

        watch.Stop();
        Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms");

        test.Save("results/result.bmp");

    }
    public static void Main(string[] args)
    {
        RemoveWhiteBoundaries obj = new RemoveWhiteBoundaries();
        obj.Run();
    }
}
