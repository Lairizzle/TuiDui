using Terminal.Gui;

Application.Init();

try
{
    Application.Run(new TuiDui.MainDialog());
}
finally
{
    Application.Shutdown();
}
