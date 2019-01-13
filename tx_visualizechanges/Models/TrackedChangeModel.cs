using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace tx_visualizechanges.Models
{
    public class TrackedChangeModel
    {
        public string DocumentName { get; set; }
        public DateTime FirstChange { get; set; } = DateTime.MaxValue;
        public List<TrackedChange> TrackedChanges { get; set; } = new List<TrackedChange>();
    }

    public class TrackedChange
    {
        public string UserName { get; set; }
        public DateTime ChangeTime { get; set; }
        public string Text { get; set; }
        public string HighlightColor { get; set; }
        public ChangeKind ChangeKind { get; set; }
        public string Image { get; set; }
    }

    public enum ChangeKind
    {
        InsertedText = 4096,
        DeletedText = 8192
    }
}