import Char

-- lmap toUpper "Hello"
lmap f [] = []
lmap f (x:xs) = f x : lmap f xs

-- mmap (\elem cont -> cont (toUpper elem))
mmap f [] = []
mmap f (x:xs) = f x (\next -> next : mmap f xs)

-- lfold (+) 0 [1..10]
lfold f z [] = z
lfold f z (x:xs) = f x (lfold f z xs)

-- mfold (\elem soFar cont -> ((+) elem (cont soFar))) 0 [1..10]
mfold f z [] = z
mfold f z (x:xs) = f x z (\next -> mfold f next xs)

lfilter f [] = []
lfilter f (x:xs) = if f x then x : lfilter f xs
	else lfilter f xs
	

map' f k x::l = f (\v -> map' f (\v' -> k (v::v')) l) x