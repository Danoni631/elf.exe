phase_1:
    call playsound

    mov ax, 0xa000
    mov es, ax
    xor di, di
    mov cx, 64000
.bg:
    in al, 0x40
    and al, 0x1F
    stosb
    loop .bg

    mov si, msg1
    call print_string_center
    inc byte [timer]
    jmp end_frame

phase_2:
    call stopsound

    mov ax, 0xa000
    mov es, ax
    xor di, di
.draw:
    mov ax, di
    mov bx, WSCREEN
    xor dx, dx
    div bx          ; ax=y, dx=x
    and al, dl
    add al, [timer]
    stosb
    cmp di, 64000
    jne .draw
    
    mov si, msg2
    call print_string_center
    inc byte [timer]
    jmp end_frame

phase_3:
    mov ax, 0xa000
    mov es, ax
    xor di, di
.draw:
    mov ax, di
    mov bx, WSCREEN
    xor dx, dx
    div bx          ; ax=y, dx=x
    xor al, dl
    add al, [timer]
    stosb
    cmp di, 64000
    jne .draw
    
    mov si, msg3
    call print_string_center
    inc byte [timer]
    jmp end_frame

phase_4:
    mov ax, 0xA000
    call Mandelbrot
    jmp end_frame