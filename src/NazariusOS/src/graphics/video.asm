; THIS FILE IS A SIMPLE VIDEO SYSTEM

%define WSCREEN 320
%define HSCREEN 200

end_frame:
    mov dx, 0x3DA
.wait:
    in al, dx
    test al, 8
    jz .wait
    
    jmp main_sequence

delay:
    mov cx, 0x07FF
wait_loop:
    loop wait_loop

    mov ah, 01h
    int 16h
    jz main_sequence

    ret