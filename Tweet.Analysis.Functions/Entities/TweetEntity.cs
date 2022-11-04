using System;
using System.Collections.Generic;

namespace Tweet.Analysis.Entities;

internal class TweetEntity
{
    public List<DateTimeOffset> CreatedAt { set; get; }
    public List<string> CreatedBy { set; get; }
    public List<string> Source { set; get; }
    public List<string> Text { set; get; }
}