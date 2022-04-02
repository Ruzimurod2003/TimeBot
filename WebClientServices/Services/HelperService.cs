using System.Text;

namespace TelegramBotService.Services
{
    public interface IHelperService
    {
        Task Consoler(bool toConsole, bool toFile = false, string message = "", string fileLocation = "");
        Task FileLog(string path, string data, string dataType = "DEFAULT");

    }

    public class HelperService : IHelperService
    {
        public async Task Consoler(bool toConsole, bool toFile = false, string message = "", string fileLocation = "")
        {
            if (toConsole)
            {
                Console.WriteLine(message);
            }

            if (toFile)
            {
                await FileLog(fileLocation, message, "API_FULL");
            }

            await Task.FromResult(0);
        }

        public async Task FileLog(string path, string data, string dataType = "DEFAULT")
        {
            IList<FileStream> sourceStreams = new List<FileStream>();
            try
            {
                path = Path.Combine(path);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                string t;
                int seconds;
                string todaydate;
                DateTime dt = DateTime.Now;
                seconds = dt.Second;
                todaydate = dt.ToString("yyyyMMddHH");
                if (!Equals(seconds, dt.Second))
                {
                    seconds = dt.Second;
                }

                t = dt.ToString("O");
                IList<Task> writeTaskList = new List<Task>();
                byte[] encodedText = Encoding.Unicode.GetBytes(data);

                FileStream sourceStream =
                    new FileStream(
                        $"{path}{dataType}-{todaydate}.txt",
                        FileMode.Create, FileAccess.Write, FileShare.None,
                        bufferSize: 4096, useAsync: true);
                Task writeTask = sourceStream.WriteAsync(encodedText, 0, encodedText.Length);
                sourceStreams.Add(sourceStream);
                await Task.WhenAll(writeTaskList);
            }
            catch (Exception ee)
            {
                Console.WriteLine(ee.Message);
            }
            finally
            {
                foreach (FileStream sourceStream in sourceStreams)
                {
                    sourceStream.Close();
                }
            }
        }
    }
}
