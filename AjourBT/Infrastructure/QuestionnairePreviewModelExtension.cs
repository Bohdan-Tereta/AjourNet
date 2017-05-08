using AjourBT.Domain.ViewModels;
using PDFjet.NET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace AjourBT.Infrastructure
{
    public static class QuestionnairePreviewModelExtension
    {

        public static MemoryStream GeneratePDF(this QuestionnairePreviewModel model)
        { 
            MemoryStream pdfMemoryStream = new MemoryStream();
            PDF pdf = new PDF(pdfMemoryStream);
            Font titleFont = new Font(pdf, new FileStream(AppDomain.CurrentDomain.BaseDirectory + "/Resources/DejaVuLGCSerif.ttf.deflated", FileMode.Open, FileAccess.Read)); 
            titleFont.SetSize(22f);
            Font questionTitleFont = new Font(pdf, new FileStream(AppDomain.CurrentDomain.BaseDirectory + "/Resources/DejaVuLGCSerif.ttf.deflated", FileMode.Open, FileAccess.Read)); 
            questionTitleFont.SetSize(18f);
            Font textLineFont = new Font(pdf, new FileStream(AppDomain.CurrentDomain.BaseDirectory + "/Resources/DejaVuLGCSerif.ttf.deflated", FileMode.Open, FileAccess.Read)); 
            textLineFont.SetSize(14f); 

            Table questionTable = new Table();
            List<List<Cell>> tableData = new List<List<Cell>>();
 
            Cell title = new Cell(titleFont, model.Title);
            title.SetTextAlignment(Align.CENTER);
            tableData.Add(new List<Cell> { title });

            foreach (KeyValuePair<String, String> question in model.GeneratedQuestionaryForPdf)
            {
                Cell questionTitle = new Cell(questionTitleFont, "Question " + question.Key);
                tableData.Add(new List<Cell> { questionTitle }); 
                string[] splittedQuestion = question.Value.Split(new string [] {"\\n"}, StringSplitOptions.None);
                foreach (string textLine in splittedQuestion)
                {
                    tableData.Add(new List<Cell> { new Cell(textLineFont, textLine) });
                }
                tableData.Add(new List<Cell> { new Cell(textLineFont, "") });
            }
            questionTable.SetData(tableData);
            questionTable.AutoAdjustColumnWidths(); 
            questionTable.SetNoCellBorders();
            questionTable.SetPosition(25, 10);
            questionTable.SetColumnWidth(0, A4.PORTRAIT[0] - 50);
            questionTable.WrapAroundCellText(); 

            Page page = new Page(pdf, A4.PORTRAIT);
            while (true)
            {
                questionTable.DrawOn(page);
                if (!questionTable.HasMoreData())
                {
                    questionTable.ResetRenderedPagesCount();
                    break;
                }
                page = new Page(pdf, A4.PORTRAIT);
            } 
            pdf.Flush();
            return pdfMemoryStream; 
        }

    }
}