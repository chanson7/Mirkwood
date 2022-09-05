using System.Collections.Generic;

public class WebsocketMessage
{
    public string action { get; set; }
    public Dictionary<string, object> data { get; set; }

    public WebsocketMessage(string action, Dictionary<string, object> data)
    {
        this.action = action;
        this.data = data;
    }

}
