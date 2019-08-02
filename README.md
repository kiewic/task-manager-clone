# Windows Task Manager Clone App

This .NET console app imitates the Windows Task Manager Details tab.

The following values (in kilobytes) are displayed for each running process:

1. Name
2. PID
3. Working set (memory)
4. Memory (private working set)
5. Commit size

WMI classes used:

* [Win32_Process class](https://docs.microsoft.com/en-us/windows/desktop/CIMWin32Prov/win32-process)
* [Win32_PerfRawData_PerfProc_Process](https://stackoverflow.com/a/15938001/27211)
