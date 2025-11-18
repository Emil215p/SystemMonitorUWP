// ConsoleApplication1.cpp : Defines the entry point for the console application.
//

#include "pch.h"

int main(int argc, char **argv)
{
    std::cout << "Console application launched!" << std::endl;

    if (argc > 1)
    {
        std::cout << "Argument: " << argv[1] << std::endl;
    }

    return 5;
}
