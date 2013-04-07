﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using AshMind.Extensions;
using DependencyInjection.TableGenerator.Data;

namespace DependencyInjection.TableGenerator.Outputs {
    public class HtmlTableOutput : IFeatureTableOutput {
        public void Write(DirectoryInfo directory, IEnumerable<FeatureTable> tables) {
            var builder = new StringBuilder();

            builder.AppendLine("<!DOCTYPE html>")
                   .AppendLine("<html>")
                   .AppendLine("  <head>")
                   .AppendLine("    <title>.NET DI Feature Tests</title>")
                   .AppendLine("    <link rel='stylesheet' href='FeatureTests.css' />")
                   .AppendLine("  </head>")
                   .AppendLine("  <body>")
                   .Append("    <div class='note-firefox'>Note: CSS used in this file is not supported by Firefox due to ")
                   .AppendLine("<a href='https://bugzilla.mozilla.org/show_bug.cgi?id=35168'>Bug 35168</a> (13 years, really?).</div>")
                   .AppendLine("    <div class='note'>Note: This file has been generated by TableGenerator and should not be edited manually.</div>");
            
            AppendTables(builder, tables);
            builder.AppendLine("  </body>")
                   .AppendLine("</html>");

            File.WriteAllText(Path.Combine(directory.FullName, "FeatureTests.html"), builder.ToString());

            // obviously the right way would be to embed it as a resource, but IMHO it is good enough for
            // this type of project
            File.Copy(@"Outputs\FeatureTests.css", Path.Combine(directory.FullName, "FeatureTests.css"), true);
        }

        private void AppendTables(StringBuilder builder, IEnumerable<FeatureTable> tables) {
            foreach (var table in tables) {
                builder.AppendLine("    <table>")
                       .AppendFormat("      <caption class='{2}'>{0}{1}</caption>", WebUtility.HtmlEncode(table.Name), CreateDetailsElement(table.Description), GetDetailsClass(table.Description)).AppendLine();
                
                builder.AppendLine("      <tr>")
                       .AppendLine("        <th>Framework</th>");
                foreach (var feature in table.Features) {
                    builder.AppendFormat("        <th class='{2}'>{0}{1}</th>", feature.Name, CreateDetailsElement(feature.Description), GetDetailsClass(feature.Description)).AppendLine();
                }
                builder.AppendLine("      </tr>");

                foreach (var row in table.GetRows()) {
                    builder.AppendLine("      <tr>")
                           .AppendFormat("        <td>{0}</td>", row.Item1.FrameworkName).AppendLine();
                    foreach (var cell in row.Item2) {
                        AppendCell(builder, cell);
                    }
                    builder.AppendLine("      </tr>");
                }
                builder.AppendLine("    </table>");
            }
        }

        private static void AppendCell(StringBuilder builder, FeatureCell cell) {
            var details = CreateDetailsElement(cell.Comment);
            var @class = cell.State.ToString().ToLowerInvariant();

            builder.AppendFormat("        <td class='{0}{3}'>{1}{2}</td>", @class, WebUtility.HtmlEncode(cell.Text), details, GetDetailsClass(cell.Comment))
                   .AppendLine();
        }

        private static string GetDetailsClass(string text) {
            return text.IsNotNullOrEmpty() ? " with-details" : "";
        }

        private static string CreateDetailsElement(string text) {
            if (text.IsNullOrEmpty())
                return "";

            var content = WebUtility.HtmlEncode(NormalizeDetailsContent(text));
            return "<div class='details'>" + content + "</div>";
        }

        private static string NormalizeDetailsContent(string description) {
            var normalized = description ?? "";
            // replace all single new lines with spaces
            normalized = Regex.Replace(normalized, @"([^\r\n]|^)(?:\r\n|\r|\n)([^\r\n]|$)", "$1 $2");

            // collapse all spaces
            normalized = Regex.Replace(normalized, @" +", @" ");

            // remove all spaces at start/end of the line
            normalized = Regex.Replace(normalized, @"^ +| +$", "", RegexOptions.Multiline);
            return normalized;
        }
    }
}
