# Microsoft.Bot.Builder.FormFlow v4

Example [Form Flow v4](https://formflowv4sdk2.azurewebsites.net/) bot

netstandard2.0 port of https://github.com/Microsoft/BotBuilder-v3/tree/master/CSharp/Library/Microsoft.Bot.Builder/FormFlow

**FormDialog inherits from ComponentDialog**

  - IDialog<T>.StartAsync changed to ComponentDialog.BeginDialogAsync
  
  - MessageReceived called by ComponentDialog.ContinueDialogAsync and .BeginDialogAsync

FormFlow dialogs can be added to a DialogSet:
```cs
_dialogs.Add(FormDialog.FromForm(SandwichOrder.BuildForm));
```
[SandwichOrder](https://github.com/EricDahlvang/Microsoft.Bot.Builder.FormFlow/blob/master/Sample/Microsoft.Bot.Builder.TestBot/Dialogs/SandwichOrder.cs#L36)
 > ported from v3 BotBuilder [SandwichOrder](https://github.com/Microsoft/BotBuilder-v3/blob/master/CSharp/Samples/SimpleSandwichBot/Sandwich.cs#L33) FormFlow dialog

Or added to a component dialog:
```cs
public HotelsDialog() : base(nameof(HotelsDialog))
{
    var hotelsFormDialog = FormDialog.FromForm(this.BuildHotelsForm, FormOptions.PromptInStart);
    base.AddDialog(hotelsFormDialog);
}
```
[HotelsDialog](https://github.com/EricDahlvang/Microsoft.Bot.Builder.FormFlow/blob/master/Sample/Microsoft.Bot.Builder.TestBot/Dialogs/HotelsDialog.cs#L24)
 > ported from v3 BotBuilder-Samples [HotelsDialog](https://github.com/Microsoft/BotBuilder-Samples/blob/v3-sdk-samples/CSharp/core-MultiDialogs/Dialogs/HotelsDialog.cs) 


## Repository Instructions

After cloning:

- cd Microsoft.Bot.Builder.FormFLow
- Open Microsoft.Bot.Builder.FormFlow.sln
- Set Microsoft.Bot.Builder.TestBot as StartUp Project



