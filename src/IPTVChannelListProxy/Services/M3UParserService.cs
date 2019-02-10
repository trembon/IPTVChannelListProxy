using IPTVChannelListProxy.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IPTVChannelListProxy.Services
{
    public interface IM3UParserService
    {
        IEnumerable<Channel> Parse(string m3u);
    }

    public class M3UParserService : IM3UParserService
    {
        private static Regex idRegex = new Regex("tvg-id=\\\"(.*?)\\\"", RegexOptions.Compiled);
        private static Regex logoRegex = new Regex("tvg-logo=\\\"(.*?)\\\"", RegexOptions.Compiled);
        private static Regex groupRegex = new Regex("group-title=\\\"(.*?)\\\"", RegexOptions.Compiled);

        public IEnumerable<Channel> Parse(string m3u)
        {
            List<Channel> channels = new List<Channel>();

            string[] lines = m3u.Split(new string[] { Environment.NewLine, "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].StartsWith("#EXTINF:"))
                {
                    Channel channel = new Channel();

                    string[] data = lines[i].Split(',',  StringSplitOptions.RemoveEmptyEntries);
                    channel.Name = data.Last().Trim();
                    if (!char.IsLetterOrDigit(channel.Name[0]))
                        continue;

                    var idMatch = idRegex.Match(lines[i]);
                    if (idMatch.Success && idMatch.Groups.Count > 1)
                        channel.ChannelID = idMatch.Groups.Last().Value.Trim();

                    var logoMatch = logoRegex.Match(lines[i]);
                    if (logoMatch.Success && logoMatch.Groups.Count > 1)
                        channel.Logo = logoMatch.Groups.Last().Value.Trim();

                    var groupMatch = groupRegex.Match(lines[i]);
                    if (groupMatch.Success && groupMatch.Groups.Count > 1)
                        channel.Group = groupMatch.Groups.Last().Value.Trim();

                    if (string.IsNullOrWhiteSpace(channel.ChannelID))
                        channel.ChannelID = channel.Name;

                    do
                    {
                        i++;
                        if (i >= lines.Length)
                            break;

                        channel.Url = lines[i].Trim();
                    } while (!channel.Url.StartsWith("http", StringComparison.OrdinalIgnoreCase));

                    if (channel.Url != null)
                        channels.Add(channel);
                }
            }

            return channels;
        }
    }
}
