# Arbol de comportamientos (Behaviour-tree)
Esta herramienta es una implmentación basica, algunos conceptos no estan recreados de forma correcta. La propuesta es tener una base para utilizar en proyectos de Unity3d.

# Estados (States)
Los estados son lo elementos basicos de un arbol de comportamiento, conocidos como `Nodos`.

### Selector (OR)
Este `nodo` es una evaluación `or`, se ejecutaran todos sus hijos y si solo uno de ellos termina de forma correcta la ejecucion del mismo sera valida. 

### Sequence (AND)
Este `nodo` es una evaluación `and`, se ejecutaran de forma lineal, si uno de ellos falla el nodo finalizara la ejecución y fallara.

### Parallel (While)
Ese `nodo` ejecutara todos los nodos hijos de forma paralela "lineal por tick". cuando se terminen de ejecutar todos los nodos este finalizara con exito. 

Existen 3 formas de salir

* <b>ParallelMode.ExitWithFirstSuccess</b> Termina si se finaliza con exito 1 de los hijos. 
* <b>ParallelMode.ExitWithFirstFail</b> Termina si se finaliza con error 1 de los hijos. 
* <b>ParallelMode.ExitFirstComplete</b> Termina si se finaliza 1 de los hijos. 


### Condición (Condition)
Evalua una funcion, si es falso este fallara, de lo contrario finalizara con exito. 

### Accion (Action)
Ejecuta una función, simple finalizara con exito.

### Esperar Tiempo (Wait)
Espera un tiempo determinado, ese se reiniciara cada vez que se entre al `nodo`. 

### Esperar Resultado (UntilBoolean)
Espera que se cumpla el estado esperado. 

# Decoradores (Decorators)
Los decoradores nos permiten agregar funcionalidades extras a los `nodos`. 

### Invertir (Invert)
Invierte el resultado de un `nodo`

### Repetir (Repeater)
Repite la ejecución de un `nodo` de 3 formas 

* <b>RepeatMode.Fail</b> Finaliza cuando falla. 
* <b>RepeatMode.Success</b> Finaliza cuando tiene exito. 
* <b>RepeatMode.Infinite</b> No finaliza nunca. 

# Ejemplo
En el siguiente ejemplo se ve un simple "CTF"

`IA.cs`
```csharp
_tree = BehaviourTreeBuilder.Selector(gameObject);
_tree
    .Repeater(x => x.Mode = RepeatMode.Infinite)
    .Sequence()
        .Parallel(x => x.Mode = ParallelMode.ExitWithFirstFail).SetName("¡Moviendo!")
            .Condition(() => FlagGame.Instance ? NodeExcecuteState.Success : NodeExcecuteState.Fail).SetName("¿Existe bandera?")
            .Condition(() => !FlagGame.Instance.Captured ? NodeExcecuteState.Success : NodeExcecuteState.Fail).SetName("¿Hay bandera disponible?")
            .Condition(() => MoveToFlag() ? NodeExcecuteState.Success : NodeExcecuteState.Continue).SetName("Caminar a la bandera")
        .End()
        .Condition(() => FlagGame.Instance.Capture(gameObject) ? NodeExcecuteState.Success : NodeExcecuteState.Fail).SetName("Capturar")
        .Action(() => _speed = Random.Range(2f, 4f)).SetName("Cambiar velocidad")
        .Action(() => Score++)
    .End()
.End();
```

`Flag.cs`
```csharp
_tree = BehaviourTreeBuilder.Parallel(gameObject);
_tree
    .Repeater()
        .Parallel().SetName("Capturado")
            .Sequence()
                .UntilTrue(() => Captor).SetName("¿Tengo un captor?")
                .Action(() => Captured = true)
            .End()
            .Sequence().SetName("Nueva posición")
                .UntilTrue(() => Captured)
                .Action(NextPosition)
                .Action(Active)
            .End()
        .End()
    .End()
.End();
```

# Visualización
![IA.cs](https://i.ibb.co/LJ2Y1M5/Unity-Gs-F4-PSOIa8.png)
![Flag.cs](https://i.ibb.co/kGxfSnY/Unity-jtq-EDT6-NKU.png)