using System.Text.RegularExpressions;

namespace IPBot.Helpers;

public static partial class RegexHelper
{
    [GeneratedRegex("(?<scheme>https):\\/\\/(?<host>(?:(?:xn--(?!-)|xn-(?=-)|[A-Za-z])(?:(?:-[A-Za-z\\d]+)*-[A-Za-z\\d]+|[A-Za-z\\d]*)?\\.)*(?:xn--(?!-)|xn-(?=-)|[A-Za-z])(?:(?:-[A-Za-z\\d]+)*-[A-Za-z\\d]+|[A-Za-z\\d]*)?)(?::(?<port>\\d+))?(?<path>(?:\\/(?:[-\\p{L}\\p{N}._~]|%[0-9A-Fa-f]{2}|[!$&'()*+,;=]|:|@)*)*)(?:\\?(?<query>(?:[-\\p{L}\\p{N}._~]|%[0-9A-Fa-f]{2}|[!$&'()*+,;=]|:|@|[?/])*))?(?:#(?<fragment>(?:[-\\p{L}\\p{N}._~]|%[0-9A-Fa-f]{2}|[!$&'()*+,;=]|:|@|[?/])*))?")]
    public static partial Regex UrlRegex();

    [GeneratedRegex("<:[a-zA-Z0-9]+:([0-9]+)>")]
    public static partial Regex DiscordEmojiRegex();

    [GeneratedRegex("^((?:https?:)?\\/\\/)?((?:www|m)\\.)?((?:youtube(-nocookie)?\\.com|youtu.be))(\\/(?:[\\w\\-]+\\?v=|embed\\/|v\\/)?)([\\w\\-]+)(\\S+)?$")]
    public static partial Regex YouTubeUrlRegex();
    
    [GeneratedRegex(@"(?:https?://(?:twitter|x).com)(/(?:#!/)?(\w+)/status(es)?/(\d+))")]
    public static partial Regex TwitterLinkRegex();
}