playsound:
    pusha
    mov al, 0xB6
    out 0x43, al

    mov ax, 300
    shr al, 6
    out 0x42, al

    in al, 0x61
    or al, 3
    out 0x61, al
    popa

    ret

stopsound:
    pusha
    in al, 0x61
    and al, 0xFC
    out 0x61, al
    popa

    ret