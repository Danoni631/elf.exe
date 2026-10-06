; THIS CODE WAS THE MACHINE POWER SYSTEM, TO INDICATE THE
; COMMAND TO RESTART MACHINE

reboot:
    mov al, 0xFE
    out 0x64, al

    cli
    hlt

    jmp $