﻿var repo = new Repository();

var fooTask = repo.GetFooAsync(); // No await, continue
int barResult = await repo.GetBarAsync(); // Return control to the caller
int fooResult = await fooTask; // Again, return control to the caller

return barResult + fooResult;

class Repository
{
    public async Task<int> GetFooAsync()
    {
        // Return control to the caller (non-blocking),
        // it will return here and continue execution once async operation is finished.
        // The code "after" await we are waiting for is executed on another thread,
        // it must be, because it's executed in parallel.
        await Task.Delay(1000);
        
        return 1;
    }
    
    public async Task<int> GetBarAsync()
    {
        await Task.Delay(500);
        return 2;
    }
}