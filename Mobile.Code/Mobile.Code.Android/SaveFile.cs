﻿using Android.Content;
using Android.Graphics;
using Android.Media;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Mobile.Code.Droid;
using System;
using System.IO;
using System.Threading.Tasks;

[assembly: Xamarin.Forms.Dependency(typeof(SaveFile))]
namespace Mobile.Code.Droid
{
    class SaveFile : ISaveFile
    {
        public int GetImageRotation()
        {
            //     var wm = Android.App.Application.Context.GetSystemService(Android.Content.Context.WindowService)
            //.JavaCast<IWindowManager>();
            //     var d = wm.DefaultDisplay;
            IWindowManager windowManager = Android.App.Application.Context.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();

            SurfaceOrientation rotation = windowManager.DefaultDisplay.Rotation;
            //bool isLandscape = orientation == SurfaceOrientation.Rotation90 ||
            //    orientation == SurfaceOrientation.Rotation270;
            //return isLandscape ? DeviceOrientation.Landscape : DeviceOrientation.Portrait;


            if (rotation == SurfaceOrientation.Rotation90)
            {
                return 90;
            }
            if (rotation == SurfaceOrientation.Rotation180)
            {
                return 180;
            }
            if (rotation == SurfaceOrientation.Rotation270)
            {
                return 180;
            }
            if (rotation == SurfaceOrientation.Rotation0)
            {
                return 0;
            }
            return 0;
        }
        Bitmap LoadAndResizeBitmap(string filePath)
        {
            BitmapFactory.Options options = new BitmapFactory.Options { InJustDecodeBounds = true };
            BitmapFactory.DecodeFile(filePath, options);

            int REQUIRED_SIZE = 100;
            int width_tmp = options.OutWidth, height_tmp = options.OutHeight;
            int scale = 4;
            while (true)
            {
                if (width_tmp / 2 < REQUIRED_SIZE || height_tmp / 2 < REQUIRED_SIZE)
                    break;
                width_tmp /= 2;
                height_tmp /= 2;
                scale++;
            }

            options.InSampleSize = scale;
            options.InJustDecodeBounds = false;
            Bitmap resizedBitmap = BitmapFactory.DecodeFile(filePath, options);

            ExifInterface exif = null;
            try
            {
                exif = new ExifInterface(filePath);
                string orientation = exif.GetAttribute(ExifInterface.TagOrientation);

                Matrix matrix = new Matrix();
                switch (orientation)
                {
                    case "1": // landscape
                        break;
                    case "3":
                        matrix.PreRotate(180);
                        resizedBitmap = Bitmap.CreateBitmap(resizedBitmap, 0, 0, resizedBitmap.Width, resizedBitmap.Height, matrix, false);
                        matrix.Dispose();
                        matrix = null;
                        break;
                    case "4":
                        matrix.PreRotate(180);
                        resizedBitmap = Bitmap.CreateBitmap(resizedBitmap, 0, 0, resizedBitmap.Width, resizedBitmap.Height, matrix, false);
                        matrix.Dispose();
                        matrix = null;
                        break;
                    case "5":
                        matrix.PreRotate(90);
                        resizedBitmap = Bitmap.CreateBitmap(resizedBitmap, 0, 0, resizedBitmap.Width, resizedBitmap.Height, matrix, false);
                        matrix.Dispose();
                        matrix = null;
                        break;
                    case "6": // portrait
                        matrix.PreRotate(90);
                        resizedBitmap = Bitmap.CreateBitmap(resizedBitmap, 0, 0, resizedBitmap.Width, resizedBitmap.Height, matrix, false);
                        matrix.Dispose();
                        matrix = null;
                        break;
                    case "7":
                        matrix.PreRotate(-90);
                        resizedBitmap = Bitmap.CreateBitmap(resizedBitmap, 0, 0, resizedBitmap.Width, resizedBitmap.Height, matrix, false);
                        matrix.Dispose();
                        matrix = null;
                        break;
                    case "8":
                        matrix.PreRotate(-90);
                        resizedBitmap = Bitmap.CreateBitmap(resizedBitmap, 0, 0, resizedBitmap.Width, resizedBitmap.Height, matrix, false);
                        matrix.Dispose();
                        matrix = null;
                        break;
                }

                return resizedBitmap;
            }

            catch (IOException ex)
            {
                Console.WriteLine("An exception was thrown when reading exif from media file...:" + ex.Message);
                return null;
            }
        }


        public async Task<string> SaveFilesForCameraApi(string filename, byte[] bytes, float rotation = 0)
        {
            string filePath = string.Empty;
            await Task.Run(() =>
            {
                Bitmap originalImage = BitmapFactory.DecodeByteArray(bytes, 0, bytes.Length);
                Matrix matrix = new Matrix();

                int width = originalImage.Width;
                int height = originalImage.Height;
                if (rotation != 0)
                {
                    if (MainActivity.AppOrientation == 0)
                    {
                        matrix.PostRotate(0);
                    }
                    if (MainActivity.AppOrientation == 2)
                    {
                        matrix.PostRotate(-90);
                    }
                    if (MainActivity.AppOrientation == 3)
                    {
                        matrix.PostRotate(90);
                    }
                    if (MainActivity.AppOrientation == 4)
                    {
                        matrix.PostRotate(180);
                    }
                    matrix.PostRotate(rotation);
                }
                else
                    matrix.PostRotate(GetImageRotation());
                Bitmap resizedImage = Bitmap.CreateBitmap(originalImage, 0, 0, width, height, matrix, true);


                using (MemoryStream ms = new MemoryStream())
                {
                    resizedImage.Compress(Bitmap.CompressFormat.Jpeg, 30, ms);
                    bytes = ms.ToArray();
                }

                var documentsPath = Android.App.Application.Context.GetExternalFilesDir("").AbsolutePath; //Android.OS.Environment.StorageDirectory.AbsolutePath;
                documentsPath = documentsPath.Replace("Android/data/com.deckinspectors.mobile/files", "");
                filePath = System.IO.Path.Combine(documentsPath, "DeckInspectors");
                Directory.CreateDirectory(filePath);
                filePath = System.IO.Path.Combine(filePath, filename) + ".png";
                File.WriteAllBytes(filePath, bytes);
            });                       

            return filePath;

        }

        public async Task<string> SaveFiles(string filename, byte[] bytes)
        {

            //var documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            var documentsPath = Android.App.Application.Context.GetExternalFilesDir("").AbsolutePath;
            documentsPath = documentsPath.Replace("Android/data/com.deckinspectors.mobile/files", "");
            var filePath = System.IO.Path.Combine(documentsPath, "DeckInspectors");
            //var filePath = System.IO.Path.Combine(documentsPath, filename);
            Directory.CreateDirectory(filePath);
            filePath = System.IO.Path.Combine(filePath, filename) + ".png";
            File.WriteAllBytes(filePath, bytes);
            OpenFile(filePath, filename);

            return await Task.FromResult(filePath);

        }
        public void OpenFile(string filePath, string filename)
        {

            var bytes = File.ReadAllBytes(filePath);

            //Copy the private file's data to the EXTERNAL PUBLIC location
            string externalStorageState = global::Android.OS.Environment.ExternalStorageState;
            string application = "";

            string extension = System.IO.Path.GetExtension(filePath);

            switch (extension.ToLower())
            {
                case ".doc":
                case ".docx":
                    application = "application/msword";
                    break;
                case ".pdf":
                    application = "application/pdf";
                    break;
                case ".xls":
                case ".xlsx":
                    application = "application/vnd.ms-excel";
                    break;
                case ".jpg":
                case ".jpeg":
                case ".png":
                    application = "image/jpeg";
                    break;
                default:
                    application = "*/*";
                    break;
            }
            var externalPath = global::Android.OS.Environment.ExternalStorageDirectory.Path + "/" + filename + extension;
            File.WriteAllBytes(externalPath, bytes);

            Java.IO.File file = new Java.IO.File(externalPath);
            file.SetReadable(true);
            //Android.Net.Uri uri = Android.Net.Uri.Parse("file://" + filePath);
            Android.Net.Uri uri = Android.Net.Uri.FromFile(file);
            Intent intent = new Intent(Intent.ActionView);
            intent.SetDataAndType(uri, application);
            intent.SetFlags(ActivityFlags.ClearWhenTaskReset | ActivityFlags.NewTask);

            try
            {
                Xamarin.Forms.Forms.Context.StartActivity(intent);
            }
            catch (Exception)
            {
                Toast.MakeText(Xamarin.Forms.Forms.Context, "Photo saved successfully.", ToastLength.Short).Show();
            }
        }
    }
}