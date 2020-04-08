using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JLPTWeb.ViewModels
{
    public class VocSenViewModel
    {
        public VocSenViewModel()
        {
        }
        public VocSenViewModel(string vocContent, string vocContentFormat, string vocKanji, string vocHiragana,
            string vocKind,  string meanContent, string sentence, string senFormat,  string meanSenContent)
        {
            VocContent = vocContent;
            VocContentFormat = vocContentFormat;
            VocKanji = vocKanji;
            VocHiragana = vocHiragana;
            VocKind = VocKind;
            MeanContent = meanContent;
            Sentence = sentence;
            SenFormat = senFormat;
            MeanSenContent = meanSenContent;
        }
        public string VocContent { get; set; }
        
        public string VocContentFormat { get; set; }

        public string VocKanji { get; set; }

        public string VocHiragana { get; set; }

        public string VocKind { get; set; }

        public string MeanContent { get; set; }

        public string Sentence { get; set; }

        public string SenFormat { get; set; }

        public string MeanSenContent { get; set; }
        
    }
}