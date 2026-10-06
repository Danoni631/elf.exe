; A INPUT TEXT SYSTEM

print_string_center:
    ; Reset cursor
    mov ah, 02h
    xor bh, bh
    mov dx, 0x0C0F
    int 10h
.loop:
    lodsb
    test al, al
    jz .done
    mov ah, 0x0E
    mov bl, [timer]
    and bl, 0x0F
    add bl, 32
    int 10h
    jmp .loop
.done:
    ret