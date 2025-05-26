
using DocumentFormat.OpenXml.Packaging;
using OpenXmlPowerTools;
using System;
using System.Collections.Generic;
using System.IO;

namespace Ferreteria.Domain.Utils
{
    public static class WordTool
    {

        public static string ReplaceKeys(string templatePath, string destinationPath, Dictionary<string, string> keysToReplace)
        {
            try
            {

                string destinationFileName = Path.Combine(destinationPath, $"{new Random().Next(1111, 9999).ToString()}_{DateTime.Now.ToString("ddMMyyyy")}.docx");

                File.Copy(templatePath, destinationFileName);

                using WordprocessingDocument wordDoc = WordprocessingDocument.Open(destinationFileName, true);

                foreach (var item in keysToReplace)
                {
                    TextReplacer.SearchAndReplace(wordDoc, search: item.Key, replace: item.Value, matchCase: true);
                }

                return destinationFileName;

            }
            catch (Exception)
            {
                throw;
            }
        }



    }
}


