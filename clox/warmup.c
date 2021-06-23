#include <stdio.h>

typedef struct Node_t {
  struct Node_t* prev;
  struct Node_t* next;
  char* data;
} Node_t;

typedef struct {
  Node_t* first;
  Node_t* last;
} DoubleLinkedList_t;

int main() {
  printf("hello, world\n");

  return 0;
}