Snips NLU C#
============
Snips NLU C# wrapper library to extract meaning from text 

References
----------
- https://snips.ai/
- https://github.com/snipsco/snips-nlu
- https://github.com/snipsco/snips-nlu-rs/

Version
-------
0.64.3

x86 and x64 libraries
---------------------
The pre-built DLL files are available in there respective directories.
### Some useful resources:
- https://www.reddit.com/r/rust/comments/78vpxg/help_cross_compiling_for_32_bit_on_windows/
- https://www.reddit.com/r/rust/comments/a5guk3/cant_use_i686pcwindowsmsvc/
- https://gist.github.com/InNoHurryToCode/955d63db0d79699fed63fe18eeebf17e
- https://github.com/japaric/rust-cross#the-target-triple

### Building the DLLs
#### For Windows
To build the dll, in `Cargo.toml` set `crate-type` to `["cdylib"]`. In `\.rustup\settings.toml`, it might be necessary to change `default_host_triple` to `i686-pc-windows-msvc` or to `x86_64-pc-windows-msvc`.

To check if the necessary toolchains are installed, run `rustup toolchain list`.
- x86 build  
  If necessary, run the following to install the toolchain:
  ```
  rustup install stable-i686-pc-windows-msvc
  ```
  Then run:
  ```
  cargo build --release --target=i686-pc-windows-msvc
  ```

- x64 build
  
  If necessary, run the following to install the toolchain:
  ```
  rustup install stable-x86_64-pc-windows-msvc
  ```
  Then run:
  ```
  cargo build --release --target=x86_64-pc-windows-msvc
  ```

Code Examples
-------------
Load a Model from a folder:
``` C#
using (var snipsNLUEngine = SnipsNLUEngine.CreateFromDirectory(@"Data\Tests\Models\nlu_engine"))
{
    IntentClassifierResult[] intents = snipsNLUEngine.GetIntents("Can you make 3 cups of coffee?");
    Slot[] slots = snipsNLUEngine.GetSlots("Can you make 3 cups of coffee?", intents[0].IntentName);
    // or
    IntentParserResult parsed = snipsNLUEngine.Parse("Can you make 3 cups of coffee?");
    Console.WriteLine(parsed);
}
```
or 
``` C#
using (var snipsNLUEngine = new SnipsNLUEngine(@"Data\Tests\Models\nlu_engine"))
{
    IntentClassifierResult[] intents = snipsNLUEngine.GetIntents("Can you make 3 cups of coffee?");
    Slot[] slots = snipsNLUEngine.GetSlots("Can you make 3 cups of coffee?", intents[0].IntentName);
    // or
    IntentParserResult parsed = snipsNLUEngine.Parse("Can you make 3 cups of coffee?");
    Console.WriteLine(parsed);
}
```

Load a Model from a Zip file:
``` C#
using (var snipsNLUEngine = SnipsNLUEngine.CreateFromZip(@"Data\Tests\Models\nlu_engine.zip"))
{
    IntentParserResult parsed = snipsNLUEngine.Parse("Can you make 3 cups of coffee?");
    Console.WriteLine(parsed);
}
```
Output will be:
```
Can you make 3 cups of coffee?
        MakeCoffee (61.43%)
                '3 (Number)', '3', snips/number, number_of_cups @ [13;14]
```

TO DO list
----------

- [x] Load model from zip file
- [ ] Create `CStringArray` class

License
-------
