using System.Collections;
using System.Collections.Generic;

public class LyricLine
{
    public double time { get; set; }
    public string text { get; set; }

    public LyricLine(double time, string text)
    {
        this.time = time;
        this.text = text;
    }
}
