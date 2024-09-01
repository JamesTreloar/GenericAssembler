addi    $0,     $1,     1
addi    $0,     $2,     10
LOOP_START:
beq     $1,     $2,     LOOP_END
addi    $1,     $1,     1
b       LOOP_START
LOOP_END: