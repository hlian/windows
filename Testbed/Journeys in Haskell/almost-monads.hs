type Funt s a = s -> (a,s)

plumb :: Funt state a -> (a -> Funt state a') -> Funt state a'
boo `plumb` transformer = \s -> (uncurry transformer) (boo s)

-- (a -> (b,c)) -> (b -> c -> d) -> a -> d
-- (state -> (a,s))
-- Funt s a -> (b -> c -> d) -> a -> d

addRec :: Int -> String
	-> (Bool,String)
addRec record db = (True, db)

delRec :: Int -> String
	-> (Bool,String)
delRec record db = (True, db)

replumb :: a -> Funt s a
replumb a = \s -> (a,s)

newDB :: Funt String Bool
newDB = let (rec1, rec2, rec3) = (1,1,1) in
	(addRec rec1) `plumb` \bool1 ->
	(addRec rec2) `plumb` \bool2 -> \s3 -> (bool1 && bool2 && True, s3)
-- \s4 -> (bool1 && True && True, s4)

findBob doQuery database query queryFactory = case doQuery database query of
	Nothing -> Nothing
	Just r1 -> case doQuery database (queryFactory r1) of
		Nothing -> Nothing
		Just r2 -> case doQuery database (queryFactory r2) of
			Nothing -> Nothing
			Just r3 -> Just r3

findLove doQuery database query queryFactory =
	let r1 = doQuery database query;
		r2 = doQuery database (queryFactory r1);
		r3 = doQuery database (queryFactory r2)
	in doQuery database (queryFactory r3)
	
doRoots num =
	let {
		r1 = num + 1;
		r2 = r1 + 2;
		r3 = r2 + 3;
	}
	in r3