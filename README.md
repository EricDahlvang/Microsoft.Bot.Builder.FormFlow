# Microsoft.Bot.Builder.FormFlow

netstandard2.0 port of https://github.com/Microsoft/BotBuilder/tree/master/CSharp/Library/Microsoft.Bot.Builder/FormFlow

**FormDialog inherits from ComponentDialog**

  - IDialog<T>.StartAsync changed to ComponentDialog.BeginDialogAsync
  
  - MessageReceived called by ComponentDialog.ContinueDialogAsync and .BeginDialogAsync

FormFlow dialogs can be added to a DialogSet:

[SandwichOrder](https://github.com/EricDahlvang/Microsoft.Bot.Builder.FormFlow/blob/master/Sample/Microsoft.Bot.Builder.TestBot/Dialogs/SandwichOrder.cs#L36)
```cs
_dialogs.Add(FormDialog.FromForm(SandwichOrder.BuildForm));
```
Or added to a component dialog:

[HotelsDialog](https://github.com/EricDahlvang/Microsoft.Bot.Builder.FormFlow/blob/master/Sample/Microsoft.Bot.Builder.TestBot/Dialogs/HotelsDialog.cs#L24)
```cs
public HotelsDialog() : base(nameof(HotelsDialog))
{
    var hotelsFormDialog = FormDialog.FromForm(this.BuildHotelsForm, FormOptions.PromptInStart);
    base.AddDialog(hotelsFormDialog);
}
```

## Repository Instructions

After cloning:

- cd Microsoft.Bot.Builder.FormFLow
- git submodule init
- git submodule update --recursive --remote
- Open Microsoft.Bot.Builder.FormFlow.sln
- Set Microsoft.Bot.Builder.TestBot as StartUp Project

**Remote Dependencies**

  https://github.com/Microsoft/BotBuilder-dotnet
  
  https://github.com/EricDahlvang/nChronic/tree/netstandard2.0 
> (PR submitted: https://github.com/robertwilczynski/nChronic/pull/31 )

