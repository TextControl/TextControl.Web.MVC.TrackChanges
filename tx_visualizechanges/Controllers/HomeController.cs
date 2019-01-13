using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using tx_visualizechanges.Models;
using TXTextControl;

namespace tx_visualizechanges.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            // set document name to dummy document
            string sDocumentName = Server.MapPath("~/template.docx");

            // create a new TrackedChangeModel
            TrackedChangeModel model = new TrackedChangeModel() {
                DocumentName = sDocumentName
            };
        
            // use a temporary ServerTextControl to retrieve the
            // changes and to create the thumbnails
            using (ServerTextControl tx = new ServerTextControl())
            {
                tx.Create();

                // load the document
                tx.Load(sDocumentName, StreamType.WordprocessingML);

                // loop through all text parts including headers and footers
                foreach (IFormattedText textPart in tx.TextParts)
                {
                    // lkoop through all changes
                    foreach (TXTextControl.TrackedChange trackedChange in textPart.TrackedChanges)
                    {
                        if (trackedChange.ChangeTime < model.FirstChange) // get first change time stamp
                            model.FirstChange = trackedChange.ChangeTime;

                        // create a new TrachedChange object
                        Models.TrackedChange tc = new Models.TrackedChange()
                        {
                            ChangeTime = trackedChange.ChangeTime,
                            HighlightColor = ColorTranslator.ToHtml(trackedChange.HighlightColor),
                            ChangeKind = (Models.ChangeKind)trackedChange.ChangeKind,
                            Text = trackedChange.Text,
                            UserName = (trackedChange.UserName == "") ? "Unknown User" : trackedChange.UserName
                        };

                        // highlight the change in the document
                        trackedChange.ScrollTo();
                        trackedChange.Select();
                        textPart.Selection.TextBackColor = Color.Yellow;

                        // create a thumbnail of the page and the highlighted change
                        tc.Image = ImageToBase64(
                            tx.GetPages().GetItem().GetImage(100, Page.PageContent.All)
                        , ImageFormat.Jpeg);

                        // add the change to the model
                        model.TrackedChanges.Add(tc);
                    }
                }
            }

            return View(model);
        }

        public string ImageToBase64(System.Drawing.Image image, ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, format);
                byte[] imageBytes = ms.ToArray();
                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
        }
    }
}