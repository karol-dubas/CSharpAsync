﻿using System.Runtime.CompilerServices;

string value = await GetAsync(); // start async state machine
Console.WriteLine(value);

// Every async method will look like this, only different method builders are used
Task<string> GetAsync() // no async
{
    var stateMachine = new StateMachine
    {
        State = -1, // initial state
        MethodBuilder = AsyncTaskMethodBuilder<string>.Create()
    };
    
    stateMachine.MethodBuilder.Start(ref stateMachine); // calls StateMachine.MoveNext
    return stateMachine.MethodBuilder.Task; // Task.Status = WaitingForActivation
}

// struct only in Release mode, in Debug it's a sealed class
internal struct StateMachine : IAsyncStateMachine
{
    public int State;
    public AsyncTaskMethodBuilder<string> MethodBuilder;
    
    private TaskAwaiter _taskAwaiter;
    
    void IAsyncStateMachine.MoveNext()
    {
        try
        {
            if (State == -1) // not started yet
            {
                Console.WriteLine("Starting async operation...");

                // No await here, GetAwaiter returns TaskAwaiter, which awaits result of this asynchronous operation.
                // When it returns a value TaskAwaiter<T> is used instead.
                _taskAwaiter = Task.Delay(3_000).GetAwaiter();

                if (_taskAwaiter.IsCompleted) // lucky check
                {
                    Console.WriteLine("Async operation completed immediately");
                    State = 0;
                }
                else
                {
                    State = 0; // next time it enters MoveNext method it will instantly take a result

                    // Schedule state machine to execute when async operation is completed.
                    // It saves the state machine's state (stack -> heap) and returns control to the caller.
                    // Task Scheduler and OS is involved to resume code execution when result is available.
                    MethodBuilder.AwaitUnsafeOnCompleted(ref _taskAwaiter, ref this);
                    Console.WriteLine("State machine state moved to heap");
                    return; // state saved, leave and wait for a result
                }
            }

            if (State == 0) // task completion
            {
                // TaskAwaiter.GetResult is a blocking operation, like Task.Result, when async operation isn't completed.
                // It returns void here, but it can return a generic type.
                _taskAwaiter.GetResult();

                Console.WriteLine("Async operation completed");
                MethodBuilder.SetResult("async result");
                State = -2; // finished
            }
        }
        catch (Exception e)
        {
            MethodBuilder.SetException(e);
            State = -2; // finished
        }
    }

    void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
    {
        MethodBuilder.SetStateMachine(stateMachine); // associate builder with state machine
    }
}