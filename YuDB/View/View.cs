using System.Text;

namespace YuDB.View
{
    public class View
    {
        private static readonly View VIEW = new View();
        public static View Create()
        {
            return VIEW;
        }
        private View() {}
        private static string ContainWithinRectangle(int width, params string[] strings)
        {
            /*
             * Assumes the string is less than 80 characters, including the spacing and the borders
             */

            var result = new StringBuilder("╔");
            for (int i = 0; i < width; i++)
                result.Append("═");
            result.Append("╗\n");

            foreach (var str in strings)
            {
                if (str.Equals("=="))
                {
                    result.Append("╠");
                    for (int i = 0; i < width; i++)
                        result.Append("═");
                    result.Append("╣\n");
                }
                else if (str.Equals("--"))
                {
                    result.Append("╟");
                    for (int i = 0; i < width; i++)
                        result.Append("─");
                    result.Append("╢\n");
                }
                else
                {
                    result.Append("║");
                    result.Append(string.Format(string.Format("{{0, -{0}}}", width), str));
                    result.Append("║\n");
                }
            }

            result.Append("╚");
            for (int i = 0; i < width; i++)
                result.Append("═");
            result.Append('╝');

            return result.ToString();
        }
        private static string Inline(params string[] strings)
        {
            List<List<string>> linesArr = new List<List<string>>();
            int maxHeight = 0;
            foreach (var str in strings) {
                var lines = str.Split("\n").ToList();
                maxHeight = Math.Max(maxHeight, lines.Count);
                linesArr.Add(lines);
            }

            var result = new StringBuilder();

            for (int i = 0; i < maxHeight; i++)
            {
                for (int j = 0; j < linesArr.Count; j++)
                {
                    if (i >= linesArr[j].Count)
                    {
                        for (int k = 0; k <= j; k++)
                        {
                            for (int x = 0; x < linesArr[k][0].Count(); x++)
                                result.Append(" ");
                            result.Append(" ");
                        }
                    }
                    else
                    {
                        result.Append(linesArr[j][i]);
                        if (j != linesArr.Count - 1)
                            result.Append(" ");
                    }
                }
                result.Append('\n');
            }

            return result.ToString();
        }
    }
}
