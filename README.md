# Java Compiler

This compiler implements common grammar used in the Java programming language such as classes, functions, simple expressions, and I/O. During development, I focused on architecting and writing clean code using techniques I learned from *Clean Code: A Handbook of Agile Software Craftsmanship* by Robert C. Martin. I read the first few chapters while working my first industry job at Daktronics and figured the techniques would fit well in this project. The compiler is the final result of multiple assignments given in SDSU's Compiler Construction course.

## How It Works

Although the compiler doesn't fully implement every feature provided in existing Java compilers, it is still a fully functioning compiler!

### The Compilation Process

1. The programmer provides a Java file.
2. The compiler parses one character at a time from the file, specifically looking for tokens. Tokens can be Java keywords like "if", "while", or "class". They can also be characters like "(", "]", or "+".
3. While parsing tokens, the compiler is actively comparing the found tokens to what it expects according to Java's grammar. An example is provided below.
   
   Java grammar for declaring a constant integer would look something like:
   ```
   ConstVarDecl -> FinalT IntT IdT = NumT ;
   ```
   Where IdT is the variable name and NumT is some integer value.
   Thus, for our example the compiler expects something like:
   ```
   final int num1 = 5;
   ```
4. If the code in the Java file does not match what the grammar expects, an error message is thrown to the programmer.
5. If everything checks out, the compiler generates a three address code (TAC) file.
6. The TAC file is then used to generate Intel x86 instructions, which are output as an Assembly file.

### Running The Assembly File

The Assembly file generated from the compiler can be assembled, linked, and loaded in an x86 MSDOS environment, such as [DOSBox](https://www.dosbox.com/). This creates an executable that allows the programmer to run the program.

## Built With
### Languages Used: 
* C#
* Java
* Assembly
### Technologies Used:
* [Visual Studio](https://visualstudio.microsoft.com/vs/community/) - Popular IDE used to develop desktop, mobile, and web applications
* [DOSBox](https://www.dosbox.com/) - DOS emulator used to run Assembly programs
