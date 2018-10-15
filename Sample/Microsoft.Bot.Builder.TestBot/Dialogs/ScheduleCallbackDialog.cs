using AdaptiveCards;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.FormFlow.Advanced;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.Bot.Builder.TestBot.Dialogs
{
    public class ScheduleCallbackDialog
    {
        public string Name { get; set; }
        [Pattern(@"(\(\d{3}\))?\s*\d{3}(-|\s*)\d{4}")]
        public string PhoneNumber { get; set; }
        
        public DateTime? RequestedDate { get; set; }

        private static AdaptiveCard GetDateCard(Activity activity)
        {
            //AdaptiveCard card = new AdaptiveCard();
            //card.Body = new List<AdaptiveElement>()
            //{
            //    new AdaptiveTextBlock()
            //    {
            //        Text = "What date would you like to meet?"
            //    },
            //    new AdaptiveDateInput()
            //    {
            //        Value = DateTime.Now.ToShortDateString(),
            //        Id = "dateInp"
            //    },
            //    new AdaptiveTimeInput()
            //    {
            //        Value = DateTime.Now.ToShortTimeString(),
            //        Id = "timeInp"
            //    }
            //};
            //card.Actions = new List<AdaptiveAction>()
            //{
            //    new AdaptiveSubmitAction()
            //    {
            //        Type = "Action.Submit",
            //        Title = "Confirm"
            //    }
            //};
            //return card;
            var useMonth = DateTime.Now;


            //activity.ChannelData <-- check for previousDate?
            //if (previousDate.HasValue)
            //    useMonth = previousDate.Value.AddMonths(1);

            //if there are only three days left in the month, use next month
            if (useMonth.AddDays(3).Month > useMonth.Month)
                useMonth = DateTime.Now.AddDays(3);
            var month = new Month(useMonth);

            var card = month.ToCard();

            card.Body.Insert(0, new AdaptiveTextBlock() { Text = "Please choose a date for the appointment:" });

            return card;
        }

        private static IFormBuilder<ScheduleCallbackDialog> GetFormbuilder()
        {
            IFormBuilder<ScheduleCallbackDialog> formBuilder = new FormBuilder<ScheduleCallbackDialog>()
                .Prompter(async (context, prompt, state, field) =>
                {
                    var preamble = context.MakeMessage();
                    var promptMessage = context.MakeMessage();
                    if (prompt.GenerateMessages(preamble, promptMessage))
                    {
                        await context.PostAsync(preamble);
                    }

                    if (field != null && field.Name == nameof(ScheduleCallbackDialog.RequestedDate))
                    {
                        var attachment = new Attachment()
                        {
                            Content = GetDateCard(context.Context.Activity),
                            ContentType = AdaptiveCard.ContentType,
                            Name = "Requested Date Adaptive Card"
                        };

                        promptMessage.Attachments.Add(attachment);
                    }

                    await context.PostAsync(promptMessage);

                    return prompt;
                }).Message("Please enter your information to schedule a callback.");

            return formBuilder;
        }

        public static IForm<ScheduleCallbackDialog> BuildForm()
        {
            IFormBuilder<ScheduleCallbackDialog> formBuilder = GetFormbuilder();

            var built = formBuilder
                .Field(nameof(Name), "What is your name?")
                .Field(nameof(PhoneNumber), "Please enter your phone number.")
                .Field(nameof(RequestedDate))
                .Confirm("Is this information correct? {*}")
                .Build();

            return built;
        }

        public static FormDialog<ScheduleCallbackDialog> GetDialog()
        {
            var dialog = FormDialog.FromForm(ScheduleCallbackDialog.BuildForm);
          
            return dialog;
        }

        class Day
        {
            public enum CalendarStatus
            {
                Available,
                Unavailable,
                Proposed
            }
            public DateTime Date { get; set; }
            public CalendarStatus Status { get; set; }

            public Day(DateTime date, DateTime firstDayOfMonth)
            {
                this.Date = date;
                if (date.Date < DateTime.Now || date.Month != firstDayOfMonth.Month)
                    this.Status = CalendarStatus.Unavailable;
                else
                    this.Status = CalendarStatus.Available;
            }
        }

        class Month
        {
            public Day[] days;
            private string monthName;
            private string[] abbreviatedDayNames = { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };
            private DateTime firstDay;

            public Month(DateTime month)
            {
                days = new Day[7 * 6];
                var actualMonth = month.Month;
                var actualYear = month.Year;
                firstDay = new DateTime(actualYear, actualMonth, 1);
                this.monthName = month.ToString("MMMM");
                var first = firstDay.AddDays(0 - firstDay.DayOfWeek); //Find the first Sunday
                for (DateTime i = first; i < first.AddDays(7 * 6); i = i.AddDays(1))
                {
                    days[(int)(i - first).TotalDays] = new Day(i, firstDay);
                }
            }

            public AdaptiveCard ToCard()
            {
                AdaptiveCard json = new AdaptiveCard();
                //var monthContainer = new AdaptiveContainer();
                //monthContainer.Style = AdaptiveContainerStyle.Default;
                //monthContainer.Items.Add(new AdaptiveTextBlock()
                //{
                //    Text = this.monthName + " →",
                //    Weight = AdaptiveTextWeight.Bolder
                //});
                //var submitJson = JsonConvert.SerializeObject(new { PreviousDate = this.firstDay });
                //monthContainer.SelectAction = new AdaptiveSubmitAction() { DataJson = submitJson };
                //json.Body.Add(monthContainer);
                json.Body.Add(new AdaptiveColumnSet());
                AdaptiveColumnSet columnSet = (AdaptiveColumnSet)json.Body[0];
                for (int cols = 0; cols < 7; cols++)
                {
                    columnSet.Columns.Add(new AdaptiveColumn());
                    for (int rows = 0; rows < 7; rows++)
                    {
                        var row = new AdaptiveTextBlock();

                        if (rows == 0)
                        {
                            columnSet.Columns[cols].Items.Add(row);
                            row.Text = abbreviatedDayNames[cols];
                            row.Weight = AdaptiveTextWeight.Bolder;
                        }
                        else
                        {
                            var day = days[(rows - 1) * 7 + cols];

                            row.Text = day.Date.Day.ToString();

                            if (day.Status == Day.CalendarStatus.Proposed)
                            {
                                columnSet.Columns[cols].Items.Add(row);
                                row.Color = AdaptiveTextColor.Accent;
                            }
                            else if (day.Status == Day.CalendarStatus.Unavailable)
                            {
                                columnSet.Columns[cols].Items.Add(row);
                                row.Color = AdaptiveTextColor.Warning;
                            }
                            else if (day.Status == Day.CalendarStatus.Available)
                            {
                                var container = new AdaptiveContainer();
                                container.Style = AdaptiveContainerStyle.Default;
                                container.Items.Add(row);

                                columnSet.Columns[cols].Items.Add(container);
                                //container.SelectAction = new AdaptiveSubmitAction() { DataJson = JsonConvert.SerializeObject(new { scheduleDate = day.Date }) };
                                container.SelectAction = new AdaptiveSubmitAction() { Data = day.Date.ToShortDateString() };
                            }
                        }

                    }
                }
                return json;
            }
        }
    }
}
