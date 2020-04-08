using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JLPTWeb.ViewModels
{
    public class SenViewModel
    {
        public SenViewModel()
        {
        }
        public SenViewModel(long sentenceId, string sentence, string senFormat, long meanId, string meanSenContent)
        {
            SentenceId = sentenceId;
            Sentence = sentence;
            SenFormat = senFormat;
            MeanId = meanId;
            MeanSenContent = meanSenContent;
        }

        public long SentenceId { get; set; }

        public string Sentence { get; set; }

        public string SenFormat { get; set; }

        public long MeanId { get; set; }

        public string MeanSenContent { get; set; }
        
    }
}