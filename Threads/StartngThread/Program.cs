﻿var mainThread = Thread.CurrentThread;
mainThread.Name = "Main thread";
mainThread.PrintBasicInfo();

bool threadCompleted = false;
var thread1 = new Thread(() =>
{
    Console.WriteLine("Doing work on another thread...");
    Thread.Sleep(1_000);
    threadCompleted = true;
});

thread1.Name = "My Thread 1";

// Application closes once all non-background threads end.
// Terminate after the end of the main thread, don't wait for this thread.
//thread1.IsBackground = true;

thread1.PrintBasicInfo();
thread1.Start(); // Assign a Thread from the OS and run it
thread1.PrintBasicInfo();

// We don't know at what point the thread will end and when main thread work can be resumed

while (!threadCompleted)
{
    Console.WriteLine($"Polling '{thread1.Name}'...");
    Thread.Sleep(200);
}

Console.WriteLine("Main thread complete");