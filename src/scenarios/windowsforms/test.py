import os
from shared.runner import TestTraits, Runner
from shared import const

EXENAME = 'windowsforms'

def main():
    traits = TestTraits(exename=EXENAME,
                        guiapp='false', 
                        )
    runner = Runner(traits)
    runner.run()
    return 1


if __name__ == "__main__":
    main()
