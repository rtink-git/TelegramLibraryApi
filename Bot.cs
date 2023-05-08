using System.Text.Json;
using System.Web;


namespace TelegramLibrary
{
    public class Bot
    {
        public long id;
        //public List<send_m> send_ml;
        string? url_root = null;
        HttpClient httpClient;

        public Bot(string token, IHttpClientFactory httpClientFactory)
        {
            var token_split = token.Split(':');
            this.id = Convert.ToInt64(token_split[0]);
            this.url_root = "https://api.telegram.org/bot" + token;
            this.httpClient = httpClientFactory.CreateClient();
        }

        public class Models
        {
            public class UpdateRoot
            {
                public bool ok { get; set; }
                public List<UpdateResult>? result { get; set; }
            }

            public class UpdateResult
            {
                public long? update_id { get; set; }
                public Message? message { get; set; }
            }

            public class Message
            {
                public int message_id { get; set; }
                //public From? from { get; set; }
                public User? from { get; set; }
                public Chat? sender_chat { get; set; }
                public Chat? chat { get; set; }
                public int date { get; set; }
                public string? text { get; set; }
                public MessageEntity[]? entities { get; set; }
                public string? caption { get; set; }
                public PhotoSize[]? photo { get; set; }
                public Video? video { get; set; }
                public Audio? audio { get; set; }
                public User? new_chat_member { get; set; }
                public String? new_chat_title { get; set; }
                public User? left_chat_member { get; set; }
                public Message? reply_to_message { get; set; }
            }

            //public class From
            //{
            //    public long id { get; set; }
            //    public string? first_name { get; set; }
            //    public string? last_name { get; set; }
            //    public string? username { get; set; }
            //}

            public class Chat
            {
                public long id { get; set; }
                public string? type { get; set; }
                public string? title { get; set; }
                public string? username { get; set; }
                public string? first_name { get; set; }
                public string? last_name { get; set; }
            }

            public class MessageEntity
            {
                public string? type { get; set; }
                public int offset { get; set; }
                public int length { get; set; }
                public string? url { get; set; }
                public User? user { get; set; }
                public string? language { get; set; }
            }

            public class PhotoSize
            {
                public string? file_id { get; set; }
                public string? file_unique_id { get; set; }
                public int width { get; set; }
                public int height { get; set; }
                public int file_size { get; set; }
            }

            public class Video
            {
                public string? file_id { get; set; }
                public int width { get; set; }
                public int height { get; set; }
                public int duration { get; set; }
                public string? mime_type { get; set; }
                public int file_size { get; set; }
            }

            public class Audio
            {
                public string? file_id { get; set; }
                public int duration { get; set; }
                public string? performer { get; set; }
                public string? title { get; set; }
                public string? mime_type { get; set; }
                public int file_size { get; set; }
            }

            public class User
            {
                public long id { get; set; }
                public bool? is_bot { get; set; }
                public string? first_name { get; set; }
                public string? last_name { get; set; }
                public string? username { get; set; }
                public string? language_code { get; set; }
                public bool? can_join_groups { get; set; }
            }

            public class DeleteMessageRoot
            {
                public bool ok { get; set; }
                public bool result { get; set; }
                public int error_code { get; set; }
                public string? description { get; set; }
            }

            public class SendMessageResponseRoot
            {
                public bool ok { get; set; }
                public Message? result { get; set; }
            }

            public class SendM
            {
                public string? chat { get; set; }
                public string? text { get; set; }
                public string? file_url { get; set; }
                public string? file_type { get; set; }
                public int? reply_to_msg_id { get; set; }
            }
        }

        public async Task<Models.UpdateRoot?> updetes(long? offset)
        {
            Models.UpdateRoot? m = null;
            //try { 
            m = JsonSerializer.Deserialize<Models.UpdateRoot>(await this.httpClient.GetStringAsync(this.url_root + "/getUpdates?offset=" + offset));
            //}
            //catch { }
            return m;
        }

        public async Task<Models.DeleteMessageRoot?> delete_message(string chat_id, int message_id)
        {
            Models.DeleteMessageRoot? m = null;
            try { m = JsonSerializer.Deserialize<Models.DeleteMessageRoot>(await this.httpClient.GetStringAsync(this.url_root + "/deleteMessage?chat_id=" + chat_id + "&message_id=" + message_id)); }
            catch { }
            return m;
        }

        public async Task<Models.SendMessageResponseRoot?> send(Models.SendM m)
        {
            Models.SendMessageResponseRoot? m_return = null;

            try
            {
                if (string.IsNullOrEmpty(m.text))
                    m.text = null;

                string? str = null;

                //var sterr = this.url_root + "/sendMessage?chat_id=" + m.chat + "&parse_mode=html&disable_web_page_preview=true" + "&text=" + HttpUtility.UrlEncode(m.text) + "&reply_to_msg_id=" + m.reply_to_msg_id;

                if (m.text != null && m.file_url == null)
                {
                    var t = this.url_root + "/sendMessage?chat_id=" + m.chat + "&parse_mode=html&disable_web_page_preview=true" + "&text=" + HttpUtility.UrlEncode(m.text) + "&reply_to_msg_id=" + m.reply_to_msg_id;

                    str = await this.httpClient.GetStringAsync(this.url_root + "/sendMessage?chat_id=" + m.chat + "&parse_mode=html&disable_web_page_preview=true" + "&text=" + HttpUtility.UrlEncode(m.text) + "&reply_to_message_id=" + m.reply_to_msg_id);
                }
                else if (m.file_url != null)
                {
                    m.file_url = m.file_url.Trim();
                    string? tp = null;
                    string? name = null;
                    byte[]? byte_array = null;
                    if (m.file_url.Contains("http"))
                    {
                        byte_array = await this.httpClient.GetByteArrayAsync(m.file_url);
                        //byte_array = await new proxy_cl.http_content(m.file_url).byte_array_async();
                        // https://cdn4.telesco.pe/file/2f5b0b3e4c.mp4?token=FX69JFKis-gYH7ZLTjUR30ShMQyFSyOTT8lUsVSWaaH5TEBw-YzOa0HEcBKoEZG46E1NnaEIwMpZ569KZmXhHqBnrV91J84sJxEkSvhWh1WEncLZFL3n3kPYrN1rQbj9pO1dCXxz2Dq0bEWsS7SzZdiYrvaMJQWqtaByR8HoZdBjpuv_Rf_FrPHIwfTmNf7owuksevlY3AF6U22ytWtM0YzgZpA2YxF3dwBsU9RSMPj0sYaaarWOiL5vlbSiI-qqtzg1wvasz4exKO_5ohedAFJegl3EhNiTgH11U8yU0WXRxyrqyawl_fyIilBaUXsfVBsBT3_h3GBUAB8UNwEZew
                        // https://cdn4.telesco.pe/file/A0r-kZUVDUXWpOdff7oAEwfzK_dNv9AzfTgXXzaewbyAS58lcUIrq7_lHBatmG-_w6n1sqTpcbAfg5gJekFew8UYOIdtVQ91u3z82cisLnd1TPCELPVZl-CyG3uJPzJf7nTHjEIrUrERFKSiNG8-etBHg86Tvm-sLGU8IjAE--TjHgcCovQO_KT_hay1dXyj-mrGqgcn1F9rFSvvoIf1C7EfmCpOPp_wftpPNPnyjSYK7AhRtEMup8NswNkuGzAdStuyDuudUoGFxJ-W2XejjiPl99J0-F00udzKMw8dfMaHE5JYgv6WD3ukePq2oqR8XEiRQOLMj4l6fC5VnQg_Fg.jpg
                        var tp_split = m.file_url.Split('?');
                        var tp_split_dot = tp_split[0].Split('.');
                        tp = tp_split_dot[tp_split_dot.Length - 1];
                        var tp_split_t = tp_split[0].Split('/');
                        name = tp_split_t[tp_split_t.Length - 1];
                    }
                    else
                    {
                        if (File.Exists(m.file_url))
                            byte_array = File.ReadAllBytes(m.file_url);
                        var tp_split = m.file_url.Split('.');
                        tp = tp_split[tp_split.Length - 1];
                        var tp_split_t = tp_split[0].Split('/');
                        name = tp_split_t[tp_split_t.Length - 1];
                    }

                    string? tpe = null;
                    if (tp == "jpg" || tp == "jpeg" || tp == "png")
                        tpe = "photo";
                    else if (tp == "mp4")
                        tpe = "video";

                    if (byte_array != null)
                    {
                        MultipartFormDataContent form = new MultipartFormDataContent();
                        var streamContent = new StreamContent(new System.IO.MemoryStream(byte_array));
                        streamContent.Headers.Add("Content-Type", "application/octet-stream");
                        streamContent.Headers.Add("Content-Disposition", "form-data; name=\"" + tpe + "\"; filename=\"" + name + "\"");
                        form.Add(streamContent, "file", name);

                        string caption = "";
                        if (m.text != null)
                            caption = "&caption=" + HttpUtility.UrlEncode(m.text);

                        var action = "sendPhoto";
                        if (tpe == "video")
                            action = "sendVideo";

                        using (var response = await this.httpClient.PostAsync(this.url_root + "/" + action + "?chat_id=" + m.chat + caption, form))
                        using (HttpContent content = response.Content)
                            str = await content.ReadAsStringAsync();

                        //str = await this.httpClient
                        //str = await new proxy_cl.http_content(this.url_root + "/" + action + "?chat_id=" + m.chat + caption).form_async(form);
                    }
                    else
                    {
                        if (m.file_url.Split('/').Length == 1)
                            if (m.file_type == "photo")
                                str = await this.httpClient.GetStringAsync(this.url_root + "/sendPhoto?chat_id=" + m.chat + "&photo=" + m.file_url + "&caption=" + HttpUtility.UrlEncode(m.text));
                            else if (m.file_type == "video")
                                str = await this.httpClient.GetStringAsync(this.url_root + "/sendVideo?chat_id=" + m.chat + "&video=" + m.file_url + "&caption=" + HttpUtility.UrlEncode(m.text));
                            else if (m.file_type == "audio")
                                str = await this.httpClient.GetStringAsync(this.url_root + "/sendAudio?chat_id=" + m.chat + "&audio=" + m.file_url + "&title=" + HttpUtility.UrlEncode(m.text));
                    }

                    if (byte_array != null && byte_array.Length < 100000)
                        m.file_type = null;
                }

                if (!string.IsNullOrEmpty(str))
                    return JsonSerializer.Deserialize<Models.SendMessageResponseRoot>(str); // Newtonsoft.Json.JsonConvert.DeserializeObject<m.send_message_response_root_json>(str);
            }
            catch { }

            return m_return;
        }

        public class user_inf
        {
            public string? username(Models.Message message)
            {
                string? user_name = null;

                if (message.from != null && !String.IsNullOrEmpty(message.from.username))
                    user_name = "@" + message.from.username;

                return user_name;
            }

            public string? user_short(Models.Message message)
            {
                string? user_name = null;

                if (message.from != null && !String.IsNullOrEmpty(message.from.first_name))
                    user_name = message.from.first_name;
                if (message.from != null && !String.IsNullOrEmpty(message.from.last_name))
                {
                    if (string.IsNullOrEmpty(user_name))
                        user_name = message.from.last_name;
                    else
                        user_name += " " + message.from.last_name;
                }

                if (message.from != null && !String.IsNullOrEmpty(message.from.username))
                    user_name = "@" + message.from.username;

                return user_name;
            }
        }

        public class send_m_optimization
        {
            public void set(Models.SendM m, bool short_title = false, bool hide_links = false)
            {
                if (m.text != null && m.text.Length > 0)
                {
                    if (hide_links)
                    {
                        var title_split = m.text.Split(' ');
                        int link_n = 0;
                        for (var i = 0; i < title_split.Length; i++)
                            if (title_split[i].Contains("http") && title_split[i][0] == 'h')
                            {
                                title_split[i] = "<a href=\"" + title_split[i] + "\">[ .:. ]</a>";
                                link_n++;
                            }
                        if (link_n > 0)
                        {
                            m.text = null;
                            foreach (var i in title_split)
                                m.text += i + " ";
                            if (m.text != null)
                                m.text = m.text.Substring(0, m.text.Length - 1);
                        }
                    }

                    if (short_title && m.text != null && m.text.Length > 512)
                    {
                        var n = 0;
                        var n_sub = 0;
                        var text_split = m.text.Split(' ');
                        for (var i = 0; i < text_split.Length; i++)
                            if (n < 512)
                            {
                                n += text_split[i].Length + 1;
                                if (text_split[i].Length > 1 && text_split[i][0] == '<' && text_split[i][1] == 'a')
                                    n_sub += 7;
                                else
                                    n_sub += text_split[i].Length + 1;
                            }
                            else
                                break;
                        m.text = m.text.Substring(0, n - 1);
                        if (n_sub > 128)
                            m.text += " ...";
                    }
                }
            }
        }
    }
}