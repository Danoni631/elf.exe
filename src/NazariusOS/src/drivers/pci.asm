PCI_CONFIG_ADDR   equ 0x0CF8
PCI_CONFIG_DATA   equ 0x0CFC

REG_MAC0          equ 0x00    ; MAC Address (6 bytes)
REG_MAR0          equ 0x08    ; Multicast Filter
REG_TSD0          equ 0x10    ; Transmit Status Descriptor 0
REG_TSAD0         equ 0x20    ; Transmit Start Address Descriptor 0
REG_RBSTART       equ 0x30    ; Receive Buffer Start Address
REG_CR            equ 0x37    ; Command Register
REG_IMR           equ 0x3C    ; Interrupt Mask Register
REG_ISR           equ 0x3E    ; Interrupt Status Register
REG_RCR           equ 0x44    ; Receive Configuration Register
REG_CONFIG1       equ 0x52    ; Configuration Register 1

section .data
    pci_io_base   dw 0x0000   ; Address I/O base for the network card
    ; (obtained via PCI)
    
section .bss
    align 4
    rx_buffer     resb 8192 + 16 + 1536
    tx_buffer     resb 1536
section .text
