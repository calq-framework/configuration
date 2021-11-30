# calq configuration
Calq Configuration is a tiny configuration framework integrated with [calq options](https://github.com/greg-chuchro/calq-options).

## Get Started
```csharp
[OptionsAttribute] // enables HelloWorld values to be overwritten by CLI options
public class HelloWorld {
    public string helloWorld;
}
````
```csharp
var helloWorld = Config.Load<HelloWorld>(); // loads from a config folder
````
