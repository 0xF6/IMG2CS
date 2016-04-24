namespace IMG2CS
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Collections.Generic;
    using System.Text;

    public class xInput
    {
        static void Main(string[] args)
        {
            List<int> arrBytes = new List<int>();
        start:
            string filePath = getCommand("Введите путь до изображения");
            string tCommand = filePath;

            if (!File.Exists(tCommand))
            {
                Console.WriteLine("Invalid path.");
                goto start;
            }

            Bitmap img = (Bitmap) Image.FromFile(tCommand);
            int h, w;

        point_size_set_h:

            tCommand = getCommand($"Исходная высота: {img.Height}{Environment.NewLine}Введите новую высоту (Нажмите enter что-бы оставить исходную высоту):");
            if (string.IsNullOrWhiteSpace(tCommand))
                tCommand = img.Height.ToString();
            if (!int.TryParse(tCommand, out h))
            {
                Console.WriteLine("Invalid Height.");
                goto point_size_set_h;
            }
        point_size_set_w:
            tCommand = getCommand($"Исходная ширина: {img.Width}{Environment.NewLine}Введите новую ширину (Нажмите enter что-бы оставить исходную ширину):");
            if (string.IsNullOrWhiteSpace(tCommand))
                tCommand = img.Width.ToString();
            if (!int.TryParse(tCommand, out w))
            {
                Console.WriteLine("Invalid Width.");
                goto point_size_set_w;
            }

            Bitmap map = ResizeImage(img, new Size(h, w));

            StringBuilder builder = new StringBuilder();
            builder.AppendLine("namespace Resource");
            builder.AppendLine("{");
            builder.AppendLine("    using System;");
            builder.AppendLine("");
            builder.AppendLine("    public class xResource");
            builder.AppendLine("    {");
            builder.AppendLine($"        public static uint[] {Path.GetFileNameWithoutExtension(filePath)} = new uint[]");
            builder.AppendLine("        {");
            builder.AppendLine("            %BYTE%");
            builder.AppendLine("        };");
            builder.AppendLine("    }");
            builder.AppendLine("}");
            StringBuilder builderByte = new StringBuilder();
            int i = 0;


            for (int j = 0; j != map.Height; j++)
            {
                for (int s = 0; s != map.Width; s++)
                {
                    arrBytes.Add(map.GetPixel(j, s).ToArgb());
                }
            }

            foreach (int arrByte in arrBytes)
            {
                builderByte.Append($"0x{arrByte.ToString("x8")}, ");
                i++;
                if (i % 10 == 0) builderByte.Append($"{Environment.NewLine}\t\t\t");
            }
            string outs = builder.ToString();
            outs = outs.Replace("%BYTE%", builderByte.ToString());
            File.WriteAllText($"{Path.GetFileNameWithoutExtension(filePath)}.cs", outs);
        }

        static Bitmap ResizeImage(Bitmap imgToResize, Size size)
        {
            try
            {
                Bitmap b = new Bitmap(size.Width, size.Height);
                using (Graphics g = Graphics.FromImage((Image)b))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.DrawImage(imgToResize, 0, 0, size.Width, size.Height);
                }
                return b;
            }
            catch
            {
                Console.WriteLine("Bitmap could not be resized");
                return imgToResize;
            }
        }

        static string getCommand(string text)
        {
            string tCommand = "";
            while (true)
            {
                Console.WriteLine(text);
                Console.Write(">");
                tCommand = Console.ReadLine();

                if(!string.IsNullOrWhiteSpace(tCommand))
                    break;
            }
            return tCommand;
        }
    }
}
